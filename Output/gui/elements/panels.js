// Base Vista Panel, allows dragging
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
}

// Register the Base Panel Element
customElements.define('vista-panel', VistaPanel);