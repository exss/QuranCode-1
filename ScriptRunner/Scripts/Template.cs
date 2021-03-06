using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.IO;
using Model;

public class MyScript : IScriptRunner
{
    /// <summary>
    /// Write your C# script inside this method.
    /// Don't change its name or parameter list
    /// This method is an example of finding all verses rhat have a prime value
    /// and a P, AP, PP, C, AC, PC or a given digit_sum (e.g 19, or 0 -> any)
    /// </summary>
    /// <param name="client">Client object holding a reference to the currently selected Book object in TextMode (eg Simplified29)</param>
    /// <param name="extra">any user parameter in the TextBox next to the EXE button (ex Frequency, LettersToJump, DigitSum target, etc)</param>
    /// <returns>true to disply back in QuranCode matching verses. false to keep script window open</returns>
    private bool MyMethod(Client client, string extra)
    {
        if (client == null) return false;
        if (client.Selection == null) return false;
        List<Verse> verses = client.Selection.Verses;

        if (client.Book != null)
        {
            // OPTIONAL: query the whole book, not just the current verses
            verses = client.Book.Verses;

            client.FoundVerses = new List<Verse>();
            foreach (Verse verse in verses)
            {
                long value = client.CalculateValue(verse);
                if (Numbers.IsAdditivePrime(value))
                {
                    bool digit_sum_ok = false;
                    int target;

                    if (extra == "")
                    {
                        target = 0; // any digit sum
                        digit_sum_ok = true;
                    }
                    else if (extra.ToUpper() == "P") // target == prime digit sum
                    {
                        digit_sum_ok = Numbers.IsPrime(Numbers.DigitSum(value));
                    }
                    else if (extra.ToUpper() == "AP") // target == additive prime digit sum
                    {
                        digit_sum_ok = Numbers.IsAdditivePrime(Numbers.DigitSum(value));
                    }
                    else if (extra.ToUpper() == "PP") // target == pure prime digit sum
                    {
                        digit_sum_ok = Numbers.IsPurePrime(Numbers.DigitSum(value));
                    }
                    else if (extra.ToUpper() == "C") // target == composite digit sum
                    {
                        digit_sum_ok = Numbers.IsComposite(Numbers.DigitSum(value));
                    }
                    else if (extra.ToUpper() == "AC") // target == additive composite digit sum
                    {
                        digit_sum_ok = Numbers.IsAdditiveComposite(Numbers.DigitSum(value));
                    }
                    else if (extra.ToUpper() == "PC") // target == pure composite digit sum
                    {
                        digit_sum_ok = Numbers.IsPureComposite(Numbers.DigitSum(value));
                    }
                    else
                    {
                        if (int.TryParse(extra, out target))
                        {
                            digit_sum_ok = (Numbers.DigitSum(value) == target);
                        }
                        else
                        {
                            return false; // to stay in the Script window
                        }
                    }

                    if (digit_sum_ok)
                    {
                        client.FoundVerses.Add(verse);
                    }
                }
            }
            return true; // to close Script window and show result
        }
        return false; // to stay in the Script window
    }

    /// <summary>
    /// Run implements IScriptRunner interface to be invoked by QuranCode application
    /// </summary>
    /// <param name="args">any number and type of arguments</param>
    /// <returns>return any type</returns>
    public object Run(object[] args)
    {
        try
        {
            if (args.Length == 2)   // ScriptMethod(Client, string)
            {
                Client client = args[0] as Client;
                string extra = args[1].ToString();

                if (client != null)
                {
                    return MyMethod(client, extra);
                }
            }
            return null;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, Application.ProductName);
            return null;
        }
    }
}
