using System;
using System.Diagnostics;
using AutomationTest;
using MonoMac.AppKit;
using MonoMac.Foundation;

public static class Program
{

    static void Pad(int depth) => Console.Write(new string(' ', depth * 2));
    static void Dump(AXUIElement element, int depth, int maxDepth)
    {

        foreach(var attr in element.GetAttributeNames())
        {
            if (AXUIElement.RecursiveAttributes.Contains(attr))
                continue;
            var value = element.GetAttribute(attr);
            if(value == null)
                continue;
            Pad(depth);
            Console.Write(attr);
            Console.Write(": ");
            using var _ = value as IDisposable;
            if (value is AXUIElement child)
            {
                if (depth == maxDepth)
                    Console.WriteLine("AXUIElement");
                else
                {
                    Console.WriteLine();
                    Dump(child, depth + 1, maxDepth);
                }
            }
            else if (value is AXElementList list)
            {
                if (depth == maxDepth)
                    Console.WriteLine($"AXUIElement[{list.Count}]");
                else if (list.Count == 0)
                    Console.WriteLine("[]");
                else
                {
                    Console.WriteLine();
                    if(list.Count == 1)
                        Dump(list[0], depth + 1, maxDepth);
                    else
                    {
                        
                        Pad(depth);
                        Console.WriteLine("[");
                        foreach (var ch in list)
                        {
                            Dump(ch, depth + 1, maxDepth);
                            Pad(depth);
                            Console.WriteLine(",");
                        }
                        Pad(depth);
                        Console.WriteLine("]");
                    }
                }
            }
            else
            {
                Console.WriteLine(value);
            }
        }
    }
    
    public static void Main(string[] args)
    {
        NSApplication.Init();

        var options = NSDictionary.FromObjectsAndKeys([NSObject.FromObject(true)],
            [NSObject.FromObject("AXTrustedCheckOptionPrompt")]);

        var trusted = AxApi.AXIsProcessTrustedWithOptions(options.Handle);
        Console.WriteLine("IsTrusted:" + trusted);
        if(!trusted)
            return;

        
        
        
        if (args.Length == 1 && args[0] == "citest")
        {
            
            foreach (var app in NSWorkspace.SharedWorkspace.RunningApplications)
            {
                var url = app.ExecutableUrl.ToString();
                if (!url.StartsWith("file:///Applications"))
                    continue;
                Console.WriteLine("=========");
                Console.WriteLine("Dumping " + app.LocalizedName);
                Console.WriteLine("=========");
                using var appElement = AXUIElement.FromPid(app.ProcessIdentifier);
                Dump(appElement, 0, 3);
            }
        }
        else
        {
            using var root = AXUIElement.FromPid(36129);
            Dump(root, 0, 3);
        }
    }
}