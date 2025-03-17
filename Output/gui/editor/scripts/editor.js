// Function to toggle the menu content when a button is clicked
function toggleContent(event, contentId) {
	event.preventDefault();

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

document.addEventListener('mousedown', function(event) {
    // If clicking in UI elements, prevent default
    if (event.target.closest('.menu-bar') || event.target.closest('.toolbar-vertical') ||
        event.target.closest('.action-bar') || event.target.closest('.play-buttons')) {
        event.preventDefault();
    }
});

document.addEventListener('click', function(event) {
    // Check if the click is outside menu-related elements before hiding
    const isMenuOrContent = event.target.closest('.menu-bar') || 
                            event.target.closest('.menu-content');
    
    if (!isMenuOrContent) {
        hideAllMenus();
    }
});


// Function to handle the selection of a button in the action bar
function selectSelectionContentButton(event) {
	// Get all action bar buttons
	const actionBarButtons = document.querySelectorAll('#selection-content button');
	
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
const selectionContentButtons = document.querySelectorAll('#selection-content button');
selectionContentButtons.forEach(button => {
	button.addEventListener('click', selectSelectionContentButton);
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