// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "utils.h"
#include "trace.h"

bool ends_with(const pal::string_t &value, const pal::string_t &suffix, bool match_case)
{
    auto cmp = match_case ? pal::strcmp : pal::strcasecmp;
    return (value.size() >= suffix.size()) &&
           cmp(value.c_str() + value.size() - suffix.size(), suffix.c_str()) == 0;
}

bool starts_with(const pal::string_t &value, const pal::string_t &prefix, bool match_case)
{
    if (prefix.empty())
    {
        // Cannot start with an empty string.
        return false;
    }
    auto cmp = match_case ? pal::strncmp : pal::strncasecmp;
    return (value.size() >= prefix.size()) &&
           cmp(value.c_str(), prefix.c_str(), prefix.size()) == 0;
}

void append_path(pal::string_t *path1, const pal::char_t *path2)
{
    if (pal::is_path_rooted(path2))
    {
        path1->assign(path2);
    }
    else
    {
        if (path1->empty() || path1->back() != DIR_SEPARATOR)
        {
            path1->push_back(DIR_SEPARATOR);
        }
        path1->append(path2);
    }
}

pal::string_t get_executable(const pal::string_t &filename)
{
    pal::string_t exe_suffix = pal::exe_suffix();
    if (exe_suffix.empty())
    {
        return filename;
    }

    if (ends_with(filename, exe_suffix, false))
    {
        // We need to strip off the old extension
        pal::string_t result(filename);
        result.erase(result.size() - exe_suffix.size());
        return result;
    }

    return filename;
}

pal::string_t strip_file_ext(const pal::string_t &path)
{
    if (path.empty())
    {
        return path;
    }
    size_t sep_pos = path.rfind(_X("/\\"));
    size_t dot_pos = path.rfind(_X('.'));
    if (sep_pos != pal::string_t::npos && sep_pos > dot_pos)
    {
        return path;
    }
    return path.substr(0, dot_pos);
}

pal::string_t get_filename_without_ext(const pal::string_t &path)
{
    if (path.empty())
    {
        return path;
    }

    size_t name_pos = path.find_last_of(_X("/\\"));
    size_t dot_pos = path.rfind(_X('.'));
    size_t start_pos = (name_pos == pal::string_t::npos) ? 0 : (name_pos + 1);
    size_t count = (dot_pos == pal::string_t::npos || dot_pos < start_pos) ? pal::string_t::npos : (dot_pos - start_pos);
    return path.substr(start_pos, count);
}

pal::string_t get_filename(const pal::string_t &path)
{
    if (path.empty())
    {
        return path;
    }

    auto name_pos = path.find_last_of(DIR_SEPARATOR);
    if (name_pos == pal::string_t::npos)
    {
        return path;
    }

    return path.substr(name_pos + 1);
}

pal::string_t get_directory(const pal::string_t &path)
{
    // Find the last dir separator
    auto path_sep = path.find_last_of(DIR_SEPARATOR);
    if (path_sep == pal::string_t::npos)
    {
        return pal::string_t(path);
    }

    return path.substr(0, path_sep);
}

const int INVALID_CHAR_COUNT = 9;
pal::char_t s_invalid_chars[INVALID_CHAR_COUNT] =
    {
        '/',
        '\\',
        '<',
        '>',
        ';',
        '"',
        '|',
        '?',
        '*'};

bool is_valid_filename(const pal::string_t &filename)
{
    if (filename.empty())
    {
        return false;
    }
    for (int i = 0; i < filename.length(); ++i)
    {
        pal::char_t ch = filename[i];
        if (ch < 31)
        {
            return false;
        }
        for (int j = 0; j < INVALID_CHAR_COUNT; ++j)
        {
            if (ch == s_invalid_chars[j])
            {
                return false;
            }
        }
    }

    return true;
}

// Try to match 0xEF 0xBB 0xBF byte sequence (no endianness here.)
bool skip_utf8_bom(pal::ifstream_t *stream)
{
    if (stream->eof() || !stream->good())
    {
        return false;
    }

    int peeked = stream->peek();
    if (peeked == EOF || ((peeked & 0xFF) != 0xEF))
    {
        return false;
    }

    unsigned char bytes[3];
    stream->read((char *)bytes, 3);
    if ((stream->gcount() < 3) ||
        (bytes[1] != 0xBB) ||
        (bytes[2] != 0xBF))
    {
        // Reset to 0 if returning false.
        stream->seekg(0, stream->beg);
        return false;
    }

    return true;
}
