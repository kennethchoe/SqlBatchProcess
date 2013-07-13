using System;

namespace SqlBatchProcess
{
    public static class BitExtensions
    {
        public static bool ToBooleanOrDefault(this String s, bool Default)
        {
            return ToBooleanOrDefault((Object)s, Default);
        }


        public static bool ToBooleanOrDefault(this Object o, bool Default)
        {
            bool returnVal = Default;
            
            if (o != null)
            {
                switch (o.ToString().ToLower())
                {
                    case "yes":
                    case "true":
                    case "ok":
                    case "y":
                        returnVal = true;
                        break;

                    case "no":
                    case "false":
                    case "n":
                        returnVal = false;
                        break;

                    default:
                        if (!Boolean.TryParse(o.ToString(), out returnVal))
                            returnVal = Default;
                        break;
                }
            }

            return returnVal;
        }
    }
}