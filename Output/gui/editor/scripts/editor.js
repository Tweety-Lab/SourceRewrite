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