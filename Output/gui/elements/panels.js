// Base Vista Panel, allows dragging and handles button events
class VistaPanel extends HTMLElement {
  constructor() {
    super();
    
    // Internal state
    this.isDragging = false;
    this.offsetX = 0;
    this.offsetY = 0;
    
    // Bind methods to this instance
    this.handleMouseDown = this.handleMouseDown.bind(this);
    this.handleMouseMove = this.handleMouseMove.bind(this);
    this.handleMouseUp = this.handleMouseUp.bind(this);
    this.handleButtonDown = this.handleButtonDown.bind(this);
    this.handleButtonUp = this.handleButtonUp.bind(this);
  }
  
  connectedCallback() {
    // Set panel style
    this.style.display = 'block';
    this.style.position = 'absolute';
    this.style.overflow = 'hidden';
    this.style.userSelect = 'none';
    this.style.minWidth = '10px';
    this.style.minHeight = '10px';

    // Center the panel on the screen
    this.centerPanel();
    
    // Add event listeners for dragging
    this.addEventListener('mousedown', this.handleMouseDown);

    // Add event listeners for button pressing
    this.addEventListener('mousedown', this.handleButtonDown);
  }

  centerPanel() {
    // Get viewport dimensions
    const viewportWidth = window.innerWidth;
    const viewportHeight = window.innerHeight;
    
    // We need to wait for the browser to render the element with its styles
    // before we can get its dimensions
    setTimeout(() => {
      // Get panel dimensions (or use default if not yet rendered)
      const panelWidth = this.offsetWidth || 300;  // Default width if not set
      const panelHeight = this.offsetHeight || 200; // Default height if not set
      
      // Calculate center position
      const left = Math.max(0, (viewportWidth - panelWidth) / 2);
      const top = Math.max(0, (viewportHeight - panelHeight) / 2);
      
      // Set position
      this.style.left = `${left}px`;
      this.style.top = `${top}px`;
    }, 0);
  }
  
  disconnectedCallback() {
    // Remove event listeners
    this.removeEventListener('mousedown', this.handleMouseDown);
    document.removeEventListener('mousemove', this.handleMouseMove);
    document.removeEventListener('mouseup', this.handleMouseUp);
    this.removeEventListener('mousedown', this.handleButtonDown);
    document.removeEventListener('mouseup', this.handleButtonUp);
    document.removeEventListener('mouseleave', this.handleButtonUp);
  }
  
  handleMouseDown(e) {
    // Only handle dragging if it's not an interactive element
    const tagName = e.target.tagName.toLowerCase();
    const interactiveElements = ['input', 'textarea', 'select', 'button', 'a'];
    
    if (interactiveElements.includes(tagName)) {
      // Allow normal interaction with form elements
      return;
    }
    
    // Only handle left mouse button
    if (e.button !== 0) return;
    
    this.isDragging = true;
    this.offsetX = e.clientX - this.getBoundingClientRect().left;
    this.offsetY = e.clientY - this.getBoundingClientRect().top;
    
    // Add global event listeners
    document.addEventListener('mousemove', this.handleMouseMove);
    document.addEventListener('mouseup', this.handleMouseUp);
    
    // Prevent text selection during drag
    e.preventDefault();
  }
  
  handleMouseMove(e) {
    if (!this.isDragging) return;
    
    // Calculate new position
    const left = e.clientX - this.offsetX;
    const top = e.clientY - this.offsetY;
    
    // Update element position
    this.style.left = `${left}px`;
    this.style.top = `${top}px`;
  }
  
  handleMouseUp() {
    this.isDragging = false;
    
    // Remove global event listeners
    document.removeEventListener('mousemove', this.handleMouseMove);
    document.removeEventListener('mouseup', this.handleMouseUp);
  }
  
  handleButtonDown(e) {
    const button = e.target.closest('button');
    if (button && this.contains(button)) {
      console.log('Button pressed');
      button.classList.add('held-button');  // Add the 'held-button' class when button is held
      
      // Listen for global events to handle releasing the button
      document.addEventListener('mouseup', this.handleButtonUp);
      document.addEventListener('mouseleave', this.handleButtonUp);
    }
  }

  handleButtonUp(e) {
    const buttons = this.querySelectorAll('button.held-button');
    buttons.forEach(button => {
      console.log('Button released');
      button.classList.remove('held-button');  // Remove the 'held-button' class
    });

    // Remove global event listeners
    document.removeEventListener('mouseup', this.handleButtonUp);
    document.removeEventListener('mouseleave', this.handleButtonUp);
  }
}

// Register the Base Panel Element
customElements.define('vista-panel', VistaPanel);
