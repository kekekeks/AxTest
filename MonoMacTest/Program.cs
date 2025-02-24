using System;
using System.Diagnostics;
using AutomationTest;
using MonoMac.AppKit;
using MonoMac.Foundation;

public static class Program
{
    public static void Main()
    {
        NSApplication.Init();

        var options = NSDictionary.FromObjectsAndKeys([NSObject.FromObject(true)],
            [NSObject.FromObject("AXTrustedCheckOptionPrompt")]);

        var trusted = AxApi.AXIsProcessTrustedWithOptions(options.Handle);
        Console.WriteLine("IsTrusted:" + trusted);
        if(!trusted)
            return;

        using var root = AXUIElement.SystemWide();
        foreach(var attr in root.GetAttributeNames())
        {
            Console.WriteLine(attr);
        }

    }
}