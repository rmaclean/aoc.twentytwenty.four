﻿using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

var changeEachRecordToBeSingleRegEx = new Regex("(\\w{1})(\\r?\\n)(\\w{1})");
var fileData = await File.ReadAllTextAsync("data.txt");
var fileRecords = changeEachRecordToBeSingleRegEx.Replace(fileData, "$1 $3");
var passports = fileRecords
    .Split('\n')
    .Where(line => !string.IsNullOrWhiteSpace(line))
    .Select(recordText => new Passport(recordText))
    .ToList();

foreach (var passport in passports)
{
    Console.WriteLine($"{passport.Valid()} = {passport}");
}

Console.WriteLine($"Valid Passports {passports.Where(passport => passport.Valid()).Count()}");