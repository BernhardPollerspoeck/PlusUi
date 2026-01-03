// PlusUi Web Interop - JavaScript functions for Blazor interoperability

window.PlusUiInterop = {
    // Reference to the .NET object for callbacks
    dotNetReference: null,
    resizeObserver: null,

    // Initialize the interop with a .NET object reference
    initialize: function (dotNetRef) {
        this.dotNetReference = dotNetRef;

        // Set up resize observer
        this.setupResizeObserver();

        // Set up visibility change detection
        this.setupVisibilityObserver();

        // Return initial viewport info
        return this.getViewportInfo();
    },

    // Get current viewport information
    getViewportInfo: function () {
        return {
            width: window.innerWidth,
            height: window.innerHeight,
            devicePixelRatio: window.devicePixelRatio || 1
        };
    },

    // Force canvas resize
    resizeCanvas: function () {
        const container = document.querySelector('.plusui-container');
        const canvas = container?.querySelector('canvas');
        if (canvas) {
            const dpr = window.devicePixelRatio || 1;

            // Use viewport dimensions
            const width = window.innerWidth;
            const height = window.innerHeight;

            // Set canvas pixel size (actual buffer size)
            canvas.width = width * dpr;
            canvas.height = height * dpr;

            // Keep CSS at 100% to fill container
            canvas.style.width = '100%';
            canvas.style.height = '100vh';

            return true;
        }
        return false;
    },

    // Get element dimensions
    getElementDimensions: function (element) {
        if (!element) {
            return { width: 0, height: 0 };
        }
        const rect = element.getBoundingClientRect();
        return {
            width: rect.width,
            height: rect.height,
            top: rect.top,
            left: rect.left
        };
    },

    // Set up resize observer for window size changes
    setupResizeObserver: function () {
        let resizeTimeout;
        const debounceDelay = 16; // ~60fps

        // Delay ResizeObserver setup to ensure DOM is ready
        const setupObserver = () => {
            const container = document.querySelector('.plusui-container');
            if (container && window.ResizeObserver) {
                this.resizeObserver = new ResizeObserver((entries) => {
                    if (resizeTimeout) {
                        clearTimeout(resizeTimeout);
                    }
                    resizeTimeout = setTimeout(() => {
                        if (this.dotNetReference) {
                            const info = this.getViewportInfo();
                            this.dotNetReference.invokeMethodAsync('OnWindowResize', info.width, info.height, info.devicePixelRatio);
                        }
                    }, debounceDelay);
                });
                this.resizeObserver.observe(container);
            } else if (!container) {
                // Retry if container not found yet
                setTimeout(setupObserver, 100);
            }
        };

        // Start trying to setup the observer
        setupObserver();

        // Fallback to window resize event for older browsers
        window.addEventListener('resize', () => {
            if (resizeTimeout) {
                clearTimeout(resizeTimeout);
            }
            resizeTimeout = setTimeout(() => {
                if (this.dotNetReference) {
                    const info = this.getViewportInfo();
                    this.dotNetReference.invokeMethodAsync('OnWindowResize', info.width, info.height, info.devicePixelRatio);
                }
            }, debounceDelay);
        });

        // Also listen for device pixel ratio changes (e.g., dragging window between monitors)
        if (window.matchMedia) {
            const mediaQuery = window.matchMedia(`(resolution: ${window.devicePixelRatio}dppx)`);
            mediaQuery.addEventListener('change', () => {
                if (this.dotNetReference) {
                    const info = this.getViewportInfo();
                    this.dotNetReference.invokeMethodAsync('OnWindowResize', info.width, info.height, info.devicePixelRatio);
                }
            });
        }
    },

    // Set up visibility change observer
    setupVisibilityObserver: function () {
        document.addEventListener('visibilitychange', () => {
            if (this.dotNetReference) {
                this.dotNetReference.invokeMethodAsync('OnVisibilityChange', !document.hidden);
            }
        });
    },

    // Focus an element
    focusElement: function (element) {
        if (element) {
            element.focus();
        }
    },

    // Blur an element
    blurElement: function (element) {
        if (element) {
            element.blur();
        }
    },

    // Keyboard handling for mobile virtual keyboard
    keyboard: {
        hiddenInput: null,

        createHiddenInput: function () {
            if (this.hiddenInput) {
                return this.hiddenInput;
            }

            const input = document.createElement('input');
            input.type = 'text';
            input.id = 'plusui-hidden-keyboard-input';
            input.autocomplete = 'off';
            input.autocapitalize = 'off';
            input.autocorrect = 'off';
            input.spellcheck = false;
            input.style.cssText = `
                position: fixed;
                left: -9999px;
                top: 50%;
                width: 1px;
                height: 1px;
                opacity: 0;
                pointer-events: none;
                z-index: -1;
            `;

            document.body.appendChild(input);
            this.hiddenInput = input;

            // Forward input events to .NET
            input.addEventListener('input', (e) => {
                if (window.PlusUiInterop.dotNetReference && e.data) {
                    window.PlusUiInterop.dotNetReference.invokeMethodAsync('OnTextInput', e.data);
                }
                input.value = ''; // Clear for next input
            });

            input.addEventListener('keydown', (e) => {
                if (window.PlusUiInterop.dotNetReference) {
                    window.PlusUiInterop.dotNetReference.invokeMethodAsync('OnKeyDown', e.key, e.code, e.shiftKey, e.ctrlKey, e.altKey, e.metaKey);
                }
            });

            input.addEventListener('keyup', (e) => {
                if (window.PlusUiInterop.dotNetReference) {
                    window.PlusUiInterop.dotNetReference.invokeMethodAsync('OnKeyUp', e.key, e.code, e.shiftKey, e.ctrlKey, e.altKey, e.metaKey);
                }
            });

            return input;
        },

        show: function (keyboardType, returnKeyType, isPassword) {
            const input = this.createHiddenInput();

            // Configure input type based on keyboard type
            switch (keyboardType) {
                case 'numeric':
                    input.type = 'number';
                    input.inputMode = 'numeric';
                    break;
                case 'decimal':
                    input.type = 'number';
                    input.inputMode = 'decimal';
                    break;
                case 'phone':
                    input.type = 'tel';
                    input.inputMode = 'tel';
                    break;
                case 'email':
                    input.type = 'email';
                    input.inputMode = 'email';
                    break;
                case 'url':
                    input.type = 'url';
                    input.inputMode = 'url';
                    break;
                default:
                    input.type = isPassword ? 'password' : 'text';
                    input.inputMode = 'text';
                    break;
            }

            // Configure return key (enterkeyhint)
            switch (returnKeyType) {
                case 'done':
                    input.enterKeyHint = 'done';
                    break;
                case 'go':
                    input.enterKeyHint = 'go';
                    break;
                case 'next':
                    input.enterKeyHint = 'next';
                    break;
                case 'search':
                    input.enterKeyHint = 'search';
                    break;
                case 'send':
                    input.enterKeyHint = 'send';
                    break;
                default:
                    input.enterKeyHint = 'enter';
                    break;
            }

            // Move temporarily into view and focus
            input.style.left = '0';
            input.style.opacity = '0';
            input.focus();

            // Move back out of view after focus
            setTimeout(() => {
                input.style.left = '-9999px';
            }, 100);
        },

        hide: function () {
            if (this.hiddenInput) {
                this.hiddenInput.blur();
            }
        }
    },

    // Haptic feedback using the Vibration API
    haptics: {
        isSupported: function () {
            return 'vibrate' in navigator;
        },

        vibrate: function (pattern) {
            if (this.isSupported()) {
                try {
                    navigator.vibrate(pattern);
                    return true;
                } catch (e) {
                    console.warn('Vibration failed:', e);
                    return false;
                }
            }
            return false;
        },

        // Predefined haptic patterns
        emit: function (feedbackType) {
            switch (feedbackType) {
                case 'light':
                    return this.vibrate(10);
                case 'medium':
                    return this.vibrate(20);
                case 'heavy':
                    return this.vibrate(30);
                case 'selection':
                    return this.vibrate(5);
                case 'success':
                    return this.vibrate([10, 50, 10]);
                case 'warning':
                    return this.vibrate([20, 50, 20]);
                case 'error':
                    return this.vibrate([30, 50, 30, 50, 30]);
                default:
                    return this.vibrate(15);
            }
        }
    },

    // Clipboard operations
    clipboard: {
        writeText: async function (text) {
            try {
                await navigator.clipboard.writeText(text);
                return true;
            } catch (e) {
                console.warn('Clipboard write failed:', e);
                return false;
            }
        },

        readText: async function () {
            try {
                return await navigator.clipboard.readText();
            } catch (e) {
                console.warn('Clipboard read failed:', e);
                return null;
            }
        }
    },

    // Request animation frame for smooth rendering
    requestRender: function (callback) {
        return requestAnimationFrame(callback);
    },

    // Cancel animation frame
    cancelRender: function (id) {
        cancelAnimationFrame(id);
    },

    // Prevent context menu on canvas (right-click)
    preventContextMenu: function (element) {
        if (element) {
            element.addEventListener('contextmenu', (e) => {
                e.preventDefault();
                return false;
            });
        }
    },

    // Prevent default wheel behavior (page scroll)
    preventWheelDefault: function (element) {
        if (element) {
            element.addEventListener('wheel', (e) => {
                e.preventDefault();
            }, { passive: false });
        }
    },

    // Prevent touch scroll/zoom on canvas
    preventTouchDefaults: function (element) {
        if (element) {
            element.style.touchAction = 'none';
            element.addEventListener('touchstart', (e) => e.preventDefault(), { passive: false });
            element.addEventListener('touchmove', (e) => e.preventDefault(), { passive: false });
            element.addEventListener('touchend', (e) => e.preventDefault(), { passive: false });
        }
    },

    // Get system color scheme preference
    getColorScheme: function () {
        if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
            return 'dark';
        }
        return 'light';
    },

    // Listen for color scheme changes
    setupColorSchemeObserver: function () {
        if (window.matchMedia) {
            window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', (e) => {
                if (this.dotNetReference) {
                    this.dotNetReference.invokeMethodAsync('OnColorSchemeChange', e.matches ? 'dark' : 'light');
                }
            });
        }
    },

    // Check if running as PWA
    isPwa: function () {
        return window.matchMedia('(display-mode: standalone)').matches ||
            window.navigator.standalone === true;
    },

    // Get safe area insets for notched devices
    getSafeAreaInsets: function () {
        const style = getComputedStyle(document.documentElement);
        return {
            top: parseInt(style.getPropertyValue('--sat') || style.getPropertyValue('env(safe-area-inset-top)') || '0') || 0,
            right: parseInt(style.getPropertyValue('--sar') || style.getPropertyValue('env(safe-area-inset-right)') || '0') || 0,
            bottom: parseInt(style.getPropertyValue('--sab') || style.getPropertyValue('env(safe-area-inset-bottom)') || '0') || 0,
            left: parseInt(style.getPropertyValue('--sal') || style.getPropertyValue('env(safe-area-inset-left)') || '0') || 0
        };
    },

    // Dispose and cleanup
    dispose: function () {
        if (this.resizeObserver) {
            this.resizeObserver.disconnect();
            this.resizeObserver = null;
        }
        this.dotNetReference = null;
        if (this.keyboard.hiddenInput) {
            this.keyboard.hiddenInput.remove();
            this.keyboard.hiddenInput = null;
        }
    }
};
