
$('#accordion').dcAccordion({
            //classParent: 'dcjq-parent',             //Class of parent menu item
            //classActive : 'active',                 // Class of active parent link
            //classArrow   : 'dcjq-icon',             // Class of span tag for parent arrows
            //classCount   : 'dcjq-count',            // Class of span tag containing count (if addCount: true)
            //classExpand : 'dcjq-current-parent',    // Class of parent li tag for auto-expand option
            eventType    : 'click',                 // Event for activating menu - options are "click" or "hover"
            //hoverDelay   : 300,                     // Hover delay for hoverIntent plugin
            //menuClose   : true,                     // If set "true" with event "hover" menu will close fully when mouseout
            //autoClose    : true,                    // If set to "true" only one sub-menu open at any time
            autoExpand   : false,                   // If set to "true" all sub-menus of parent tags with class 'classExpand' will expand on page load
            speed         : 'slow',                 // Speed of animation
            saveState    : false,                    // Save menu state using cookies
            disableLink  : true,                    // Disable all links of parent items
            showCount : false,                       // If "true" will add a count of the number of links under each parent menu item
            //cookie  : 'dcjq-accordion'
});
