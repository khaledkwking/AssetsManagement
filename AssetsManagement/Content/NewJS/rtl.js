$(document).ready(function () {
	// HTML markup implementation, overlap mode
    $( '#menu' ).multilevelpushmenu({
        direction: 'rtl',
        backItemIcon: 'fa fa-angle-left',
        groupIcon: 'fa fa-angle-right',
        containersToPush: [$( '#pushobj' )],
        collapsed: true,
        fullCollapse: false,
        mode: 'overlap',
        menuWidth:'250px',
        backText: '',
        backItemClass: 'backItemClass',
	});
});

//container: $('#menu'),                                     // Holding container.
//containersToPush: [$('#content1'), $('#content2')],        // Array of DOM elements to push/slide together with menu.
//collapsed: false,                                          // Initialize menu in collapsed/expanded mode
//menuID: 'multilevelpushmenu',                              // ID of the <nav> element.
//wrapperClass: 'multilevelpushmenu_wrapper',                // Wrapper CSS class.
//menuInactiveClass: 'multilevelpushmenu_inactive',          // CSS class for inactive wrappers.
//menu: arrMenu,                                             // JS array of menu items (if markup not provided).
//menuWidth: 0,                                              // Wrapper width (integer, '%', 'px', 'em').
//menuHeight: 0,                                             // Menu height (integer, '%', 'px', 'em').
//backText: 'Back',                                          // Text for 'Back' menu item.
//backItemClass: 'backItemClass',                            // CSS class for back menu item.
//backItemIcon: 'fa fa-angle-right',                         // FontAvesome icon used for back menu item.
//groupIcon: 'fa fa-angle-left',                             // FontAvesome icon used for menu items contaning sub-items.
//mode: 'overlap',                                           // Menu sliding mode: overlap/cover.
//overlapWidth: 40,                                          // Width in px of menu wrappers overlap
//preventItemClick: true,                                    // set to false if you do not need event callback functionality
//preventGroupItemClick: true,                               // set to false if you do not need event callback functionality
//direction: 'ltr',                                          // set to 'rtl' for reverse sliding direction
//fullCollapse: false,                                       // set to true to fully hide base level holder when collapsed
//swipe: 'both'

