using System;
using System.Diagnostics;
using AutomationTest;
using MonoMac.AppKit;
using MonoMac.Foundation;
using Newtonsoft.Json;

public static class Program
{
    static void Dump(JsonWriter wr, AXUIElement element, int depth, int maxDepth)
    {
        var names = element.GetAttributeNames();
        if (depth > 3)
        {
            if (element.GetAttribute("AXRole") == "AXWindow")
            {
                wr.WriteValue("RECUSRSION DETECTED");
                return;
            }
        }
        wr.WriteStartObject();
        var actions = element.GetActions();
        if (actions.Count > 0)
        {
            wr.WritePropertyName("_actions");
            wr.WriteStartArray();
            foreach(var a in actions)
                wr.WriteValue(a);
            wr.WriteEndArray();
        }
        
        foreach(var attr in names)
        {
            if (AXUIElement.RecursiveAttributes.Contains(attr))
                continue;
            var value = element.GetAttribute(attr);
            if(value == null)
                continue;

            wr.WritePropertyName(attr);
            using var _ = value as IDisposable;
            if (value is AXUIElement child)
            {
                if (depth == maxDepth)
                    wr.WriteValue("AXUIElement");
                else
                {
                    Dump(wr, child, depth + 1, maxDepth);
                }
            }
            else if (value is AXElementList list)
            {
                if (depth == maxDepth)
                    wr.WriteValue($"AXUIElement[{list.Count}]");
                else if (list.Count == 0)
                    wr.WriteValue("[]");
                else
                {
                    if(list.Count == 1)
                        Dump(wr, list[0], depth + 1, maxDepth);
                    else
                    {
                        wr.WriteStartArray();
                        foreach (var ch in list)
                        {
                            Dump(wr, ch, depth + 1, maxDepth);
                        }
                        wr.WriteEndArray();
                    }
                }
            }
            else
            {
                wr.WriteValue(value.ToString());
            }
        }
        wr.WriteEndObject();
    }

    static void Dump(AXUIElement element, int maxDepth)
    {
        var jw = new JsonTextWriter(Console.Out)
        {
            Formatting = Formatting.Indented
        };
        Dump(jw, element, 0, maxDepth);
        jw.Flush();
    }


    public static void Main(string[] args)
    {
        NSApplication.Init();

        var options = NSDictionary.FromObjectsAndKeys([NSObject.FromObject(true)],
            [NSObject.FromObject("AXTrustedCheckOptionPrompt")]);

        var trusted = AxApi.AXIsProcessTrustedWithOptions(options.Handle);

        if (!trusted)
        {
            Console.WriteLine("Not Trusted:");
        }



        if (args.Length == 1 && args[0] == "citest")
        {
            Console.WriteLine("Installed apps:");
            foreach (var app in Directory.GetDirectories("/Applications"))
            {
                Console.WriteLine(app);
            }

            Console.WriteLine("Starting some apps");
            Process.Start("open", "/Applications/Safari.app");
            Process.Start("open", "\"/Applications/Google Chrome.app\"");
            
            Console.WriteLine("Waiting");
            Thread.Sleep(5000);
            
            foreach (var app in NSWorkspace.SharedWorkspace.RunningApplications)
            {
                var url = app.ExecutableUrl.ToString();
                if (!url.StartsWith("file:///Applications") && !url.StartsWith("file:///System/Volumes/Preboot/"))
                {
                    Console.WriteLine("Skipping " + app.LocalizedName + " at " + url);
                    continue;
                }
                Console.WriteLine("=========");
                Console.WriteLine("Dumping " + app.LocalizedName);
                Console.WriteLine("=========");
                using var appElement = AXUIElement.FromPid(app.ProcessIdentifier);
                Dump(appElement, 3);
            }
        }
        if(args.Length == 1 && args[0] == "testapp")
        {
            foreach (var app in NSWorkspace.SharedWorkspace.RunningApplications)
            {
                if (app.ExecutableUrl.ToString().EndsWith("IntegrationTestApp"))
                {
                    using var appElement = AXUIElement.FromPid(app.ProcessIdentifier);
                    Dump(appElement, 30);
                }
            }
        }
        else
        {
            using var root = AXUIElement.FromPid(36129);
            Dump(root, 3);
        }
    }
}