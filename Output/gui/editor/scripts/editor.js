// Function to toggle the menu content when a button is clicked
function toggleContent(event, contentId) {
	// Get the button that was clicked
	const button = event.target;

	// Get the button's position relative to the document
	const rect = button.getBoundingClientRect();

	// Get all menu content elements
	const allContents = document.querySelectorAll('.menu-content');

	// Loop through all content and hide them
	allContents.forEach(content => content.style.display = 'none');

	// Get the content to display
	const selectedContent = document.getElementById(contentId);
	if (selectedContent) {
		// Position the content below the button
		selectedContent.style.left = rect.left + 'px';
		selectedContent.style.top = rect.bottom + 'px';
		selectedContent.style.display = 'flex'; // Show the selected content
	}

	// Stop the event from propagating (so it doesn't trigger the document click listener)
	event.stopPropagation();
}

// Function to hide all menu content
function hideAllMenus() {
	const allContents = document.querySelectorAll('.menu-content');
	allContents.forEach(content => content.style.display = 'none');
}

// Add a click event listener to the document to close all menus if clicked outside
document.addEventListener('click', function (event) {
	if (!event.target.closest('.menu-bar')) {
		hideAllMenus();
	}
});


// Function to handle the selection of a button in the action bar
function selectActionBarButton(event) {
	// Get all action bar buttons
	const actionBarButtons = document.querySelectorAll('.action-bar button');
	
	// Remove the 'selected-button' class from all buttons
	actionBarButtons.forEach(button => {
		button.classList.remove('selected-button');
	});
	
	// Add the 'selected-button' class to the clicked button
	event.target.classList.add('selected-button');
}

// Function to handle the selection of a button in the toolbar
function selectToolbarButton(event) {
	// Get all toolbar buttons
	const toolbarButtons = document.querySelectorAll('.toolbar-vertical button');
	
	// Remove the 'selected-button' class from all buttons
	toolbarButtons.forEach(button => {
		button.classList.remove('selected-button');
	});
	
	// Add the 'selected-button' class to the clicked button
	event.target.classList.add('selected-button');
}

// Add click event listeners to each button in the action bar
const actionBarButtons = document.querySelectorAll('.action-bar button');
actionBarButtons.forEach(button => {
	button.addEventListener('click', selectActionBarButton);
});

// Add click event listeners to each button in the toolbar-vertical
const toolbarButtons = document.querySelectorAll('.toolbar-vertical button');
toolbarButtons.forEach(button => {
	button.addEventListener('click', selectToolbarButton);
});

// Tree View
document.querySelectorAll('.caret').forEach(toggler => {
    toggler.addEventListener('click', function() {
        this.parentElement.querySelector('.nested').classList.toggle('active');
        this.classList.toggle('caret-down');
    });
});