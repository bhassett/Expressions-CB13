$(document).ready(function($){
	$('.megamenu').megaMenuReloaded({
		menu_speed_show : 300, // Time (in milliseconds) to show a drop down
		menu_speed_hide : 200, // Time (in milliseconds) to hide a drop down
		menu_speed_delay : 100, // Time (in milliseconds) before showing a drop down
		menu_effect : 'hover_slide', // Drop down effect, choose between 'hover_fade', 'hover_slide', 'click_fade', 'click_slide', 'open_close_fade', 'open_close_slide'
		menu_easing : 'jswing', // Easing Effect : 'easeInQuad', 'easeInElastic', etc.
		menu_click_outside : 1, // Clicks outside the drop down close it (1 = true, 0 = false)
		menu_show_onload : 0, // Drop down to show on page load (type the number of the drop down, 0 for none)
		menubar_trigger : 0, // Show the menu trigger (button to show / hide the menu bar), only for the fixed version of the menu (1 = show, 0 = hide)
		menubar_hide : 0 // Hides the menu bar on load (1 = hide, 0 = show)
	});
});