using MonoMac.Foundation;

namespace AutomationTest;


public class AXUIElement(IntPtr handle) : IDisposable
{
	private readonly IntPtr _handle = handle;

	public void Dispose()
    {
        //throw new NotImplementedException();
    }

	public static AXUIElement SystemWide()
	{
		return new AXUIElement(AxApi.AXUIElementCreateSystemWide());
	}
	
    public List<string> GetAttributeNames()
    {
	    var rv = new List<string>();
	    AxApi.AXUIElementCopyAttributeNames(_handle, out var arrayRef);

	    
	    using var array = new NSArray(arrayRef);
	    for (var c = 0ul; c < array.Count; c++)
	    {
		    rv.Add(NSString.FromHandle(array.ValueAt(c)));
	    }

	    return rv;
    }
}


/*- (void)interactWithUIElement:(AXUIElementRef)element
{
    NSArray* attributeNames = [UIElementUtilities attributeNamesOfUIElement:element];
    
    // populate attributes pop-up menus
    [_attributesPopup removeAllItems];
    
    // reset the contents of the elements popup
    [_elementsPopup removeAllItems];
    [_elementsPopup addItemWithTitle:@"goto"];

    if (attributeNames && [attributeNames count]){

	NSMenu *attributesPopupMenu = [_attributesPopup menu];
	
	for (NSString *attributeName in attributeNames) {
            
         //   CFTypeRef	theValue;
            
            // Grab settable field
	    BOOL isSettable = [UIElementUtilities canSetAttribute:attributeName ofUIElement:element];
            
            // Add name to pop-up menu     
	    NSMenuItem *newItem = [attributesPopupMenu addItemWithTitle:[NSString stringWithFormat:@"%@%@", attributeName, (isSettable ? @" (W)":@"")] action:nil keyEquivalent:@""];
	    [newItem setRepresentedObject:attributeName];
            
	    // If value is an AXUIElementRef, or array of them, add them to the elements popup
	    id value = [UIElementUtilities valueOfAttribute:attributeName ofUIElement:element];
	    
	    if (value) {
	    
		 One wrinkle in our UIElementUtilities methods that wrap the underlying AX C functions.  The value returned for some attributes is another UI element - an AXUIElementRef.  Because of this, to check for whether the value is an AXUIElementRef, we use CF conventions to check for type.
		
                if (CFGetTypeID((CFTypeRef)value) == AXUIElementGetTypeID()) {
		
                    NSMenuItem *item;
                    [_elementsPopup addItemWithTitle:attributeName];
                    item = [_elementsPopup lastItem];
                    [item setRepresentedObject:(id)value];
                    [item setAction:@selector(navigateToUIElement:)];
                    [item setTarget:[_elementsPopup target]];
		    
                } else if ([value isKindOfClass:[NSArray class]]) {
		
                    NSArray *values = (NSArray *)value;
                    if ([values count] > 0 && CFGetTypeID((CFTypeRef)[values objectAtIndex:0]) == AXUIElementGetTypeID()) {
                        NSMenu *menu = [[NSMenu alloc] init];
			for (id element in values) {
                            NSString *role  = [UIElementUtilities roleOfUIElement:(AXUIElementRef)element];
                            NSString *title  = [UIElementUtilities titleOfUIElement:(AXUIElementRef)element];
                            NSString *itemTitle = [NSString stringWithFormat:title ? @"%@-\"%@\"" : @"%@", role, title];
                            NSMenuItem *item = [menu addItemWithTitle:itemTitle action:@selector(navigateToUIElement:) keyEquivalent:@""];
                            [item setTarget:[_elementsPopup target]];
                            [item setRepresentedObject:element];
                        }
                        [_elementsPopup addItemWithTitle:attributeName];
                        [[_elementsPopup lastItem] setSubmenu:menu];
                        [menu release];
                    }
                }
            }
        }
    
        [_actionsPopup setEnabled:true];
        [_elementsPopup setEnabled:true];
        [self attributeSelected:NULL];
    }
    else {
    	[_attributesPopup setEnabled:false];
    	[_elementsPopup setEnabled:false];
    	[_attributeValueTextField setEnabled:false];
    	[_setAttributeButton setEnabled:false];
    }

    // populate the popup with the actions for the element
    [_actionsPopup removeAllItems];
    
    NSArray *actionNames = [UIElementUtilities actionNamesOfUIElement:element];
    
    if (actionNames && [actionNames count]) {
    
	NSMenu *actionsPopupMenu = [_actionsPopup menu];
	for (NSString *actionName in actionNames) {
            NSMenuItem *newItem = [actionsPopupMenu addItemWithTitle:actionName action:nil keyEquivalent:@""];
	    // Set the action name as the represented object as well.  That way if the title changes (maybe displaying the localized action description rather than the constant's literal value), we still have the correct value as the represented object. 
	    [newItem setRepresentedObject:actionName];
	}

    	[_actionsPopup setEnabled:true];
        [self actionSelected:NULL];
    }
    else {
    	[_actionsPopup setEnabled:false];
    	[_performActionButton setEnabled:false];
    }
    
    // set the title of the interaction window
    {
        NSString *uiElementRole  = [UIElementUtilities roleOfUIElement:element];
        NSString *uiElementTitle  = [UIElementUtilities titleOfUIElement:element];
    
        if (uiElementRole) {
            
            if (uiElementTitle && [uiElementTitle length])
                [[self window] setTitle:[NSString stringWithFormat:@"Locked on <%@ “%@”>", uiElementRole, uiElementTitle]];
            else
                [[self window] setTitle:[NSString stringWithFormat:@"Locked on <%@>", uiElementRole]];
        }
        else
            [[self window] setTitle:@"Locked on UIElement"];

    }*/