using System.Data;
using System.Linq;
using System;
using System.Text.RegularExpressions;

internal record Passport
{

    [Rule("(byr:)(?<year>\\d{4})", typeof(int))]
    public int? BirthYear { get; private set; }

    [Rule("(iyr:)(?<year>\\d{4})", typeof(int))]
    public int? IssueYear { get; private set; }

    [Rule("(eyr:)(?<year>\\d{4})", typeof(int))]
    public int? ExpirationYear { get; private set; }

    [Rule("(hgt:)(?<height>\\d+)", typeof(int))]
    public int? Height { get; private set; }

    [Rule("(hcl:)(?<hex>\\S+)", typeof(string))]
    public string HairColor { get; private set; }

    [Rule("(ecl:)(?<color>\\S+)", typeof(string))]
    public string EyeColor { get; private set; }

    [Rule("(pid:)(?<code>\\S+)", typeof(string))]
    public string PassportID { get; private set; }

    [Rule("(cid:)(?<code>\\d{2,3})", typeof(int), required: false)]
    public int? CountryID { get; private set; }

    public Passport(string record)
    {
        foreach (var prop in typeof(Passport).GetProperties())
        {
            var ruleAttr = prop.GetCustomAttributes(false).Where(attr => attr.GetType() == typeof(RuleAttribute)).FirstOrDefault();
            if (ruleAttr != null) 
            {
                var rule = ruleAttr as RuleAttribute;
                var value = rule.Expression.Set(rule.Type, record);
                if (value != null) 
                {
                    prop.SetValue(this, value);
                }
            }
        } 
    }

    public bool Valid()
    {
        foreach (var prop in typeof(Passport).GetProperties())
        {
            var ruleAttr = prop.GetCustomAttributes(false).Where(attr => attr.GetType() == typeof(RuleAttribute)).FirstOrDefault();
            if (ruleAttr != null) 
            {
                var rule = ruleAttr as RuleAttribute;
                if (rule.Required && prop.GetValue(this) == null) 
                {
                    return false;
                }
            }
        }

        return true;
    }
}

public static class RegExExtensions
{
    public static dynamic Set(this Regex regex, Type type, string input)
    {
        var matches = regex.Matches(input);
        if (matches.Any())
        {
            var group = matches[0].Groups.Values
                .Where(group => group.Name.Length > 1)
                .FirstOrDefault();

            if (group == null)
            {
                return null;
            }

            var valueAsString = group.Value;
            switch (type)
            {
                case Type s when s == typeof(String):
                    {
                        return valueAsString;
                    }
                case Type i when i == typeof(Int32):
                    {
                        return Convert.ToInt32(valueAsString);
                    }
            }
        }

        return null;
    }
}

[System.AttributeUsage(System.AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
sealed class RuleAttribute : System.Attribute
{
    public Regex Expression { get; }
    public Type Type { get; }
    public Boolean Required { get; }

    public RuleAttribute(string regex, Type type, Boolean required = true)
    {
        Type = type;
        Expression = new Regex(regex);
        Required = required;
    }

}