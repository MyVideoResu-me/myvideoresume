// Global variables to store Stripe instance, elements, and other necessary data
let stripe;
let cardElement;
let currentClientSecret; // To store the client secret received during initialization
let dotNetRef; // To store the .NET object reference for callbacks

// Initializes Stripe Elements and sets up the card form.
// dotNetObjectReference is passed to be used by confirmCardSetupOnStripe or if init itself needs to callback.
function initializeStripeElements(publishableKey, cardElementId, cardErrorsId, submitButtonId, clientSecret, dotNetObjectReference) {
    console.log("stripeInterop.js: initializeStripeElements called.");
    console.log("Publishable Key:", publishableKey ? "Received" : "Missing!");
    console.log("Card Element ID:", cardElementId);
    console.log("Card Errors ID:", cardErrorsId);
    console.log("Client Secret:", clientSecret ? "Received" : "Missing!");
    console.log(".NET Object Reference:", dotNetObjectReference ? "Received" : "Missing!");

    if (!publishableKey) {
        console.error("stripeInterop.js: Stripe Publishable Key is missing.");
        displayError(cardErrorsId, "Stripe configuration error: Publishable key is missing. Cannot initialize payment form.");
        // No dotNetRef available yet to call HandleStripeError, C# side will timeout or fail on JS call.
        return;
    }
    if (!clientSecret) {
        console.error("stripeInterop.js: Client Secret is missing for Stripe Elements initialization.");
        displayError(cardErrorsId, "Stripe configuration error: Client secret is missing. Cannot initialize payment form.");
        return;
    }
    
    dotNetRef = dotNetObjectReference; // Store for use in confirmCardSetup
    if (!dotNetRef) {
        // This is a less critical scenario as confirmCardSetupOnStripe can receive its own dotNetRef.
        // However, if other JS functions were to rely on the global dotNetRef initialized here, this would be important.
        console.warn("stripeInterop.js: .NET object reference was not provided during initialization. Callbacks initiated from JS might fail unless a valid reference is passed to them directly.");
    }

    currentClientSecret = clientSecret; // Store client secret

    try {
        stripe = Stripe(publishableKey);
    } catch (e) {
        console.error("stripeInterop.js: Error initializing Stripe with publishable key:", e.message);
        displayError(cardErrorsId, "Fatal error initializing payment provider. Please contact support.");
        if (dotNetRef) {
            dotNetRef.invokeMethodAsync('HandleStripeError', "Fatal error initializing payment provider. Stripe.js script might be blocked or publishable key is invalid.");
        }
        return;
    }
    const elements = stripe.elements();

    // Optional: Custom styling for the card element
    const style = {
        base: {
            color: '#32325d',
            fontFamily: '"Helvetica Neue", Helvetica, sans-serif',
            fontSmoothing: 'antialiased',
            fontSize: '16px',
            '::placeholder': {
                color: '#aab7c4'
            }
        },
        invalid: {
            color: '#fa755a',
            iconColor: '#fa755a'
        }
    };

    cardElement = elements.create('card', { style: style });
    
    const cardMountElement = document.getElementById(cardElementId);
    if (cardMountElement) {
        try {
            cardElement.mount(cardMountElement);
            console.log("stripeInterop.js: Card element mounted to:", cardElementId);

            // Listen for changes in the Card Element and display any errors
            cardElement.on('change', function (event) {
                const displayDiv = document.getElementById(cardErrorsId);
                if (event.error) {
                    displayDiv.textContent = event.error.message;
                    console.warn("stripeInterop.js: Card element validation error:", event.error.message);
                } else {
                    displayDiv.textContent = '';
                }
            });
        } catch (e) {
            console.error("stripeInterop.js: Error mounting card element:", e.message);
            displayError(cardErrorsId, "Error setting up card input field. Please refresh and try again.");
             if (dotNetRef) {
                dotNetRef.invokeMethodAsync('HandleStripeError', "Error mounting card element: " + e.message);
            }
            return;
        }
    } else {
        console.error("stripeInterop.js: Failed to find card mount element with ID:", cardElementId);
        // No displayError here as the cardErrorsId element might also be missing or part of the problem.
        // Inform C# side if critical and if dotNetRef is available.
        if (dotNetRef) {
            dotNetRef.invokeMethodAsync('HandleStripeError', "Frontend Error: UI element for card input (card-element) not found.");
        }
        return; // Stop if we can't mount
    }

    // The submit button is handled by the Blazor component's SaveCard method,
    // which will call confirmCardSetupOnStripe.
    // We don't need to add a click listener to the submit button here directly for form submission to Stripe.
    // However, we could disable it until Stripe.js has loaded, if desired.
    const submitButton = document.getElementById(submitButtonId);
    if (submitButton) {
        console.log("Submit button found:", submitButtonId);
        // Example: submitButton.disabled = false; (if it was initially disabled)
    } else {
        console.warn("Submit button not found with ID:", submitButtonId);
    }
    
    console.log("Stripe Elements initialized successfully.");
}

// Confirms the card setup with Stripe using the client secret and card details.
// This function is called from C# (SaveCard method).
// dotNetObjectReference is passed again here, or use the stored one (dotNetRef).
async function confirmCardSetupOnStripe(dotNetObjectReference) {
    console.log("stripeInterop.js: confirmCardSetupOnStripe called.");

    const currentDotNetRef = dotNetObjectReference || dotNetRef; // Prefer direct pass, fallback to stored

    if (!currentDotNetRef) {
        console.error("stripeInterop.js: DotNetObjectReference not available for callback in confirmCardSetupOnStripe.");
        displayError('card-errors', "A client-side error occurred. Cannot complete action. DotNet reference missing.");
        // Cannot inform C# side if currentDotNetRef is null.
        return;
    }

    if (!stripe || !cardElement) {
        console.error("stripeInterop.js: Stripe.js or Card Element not initialized. Call initializeStripeElements first.");
        displayError('card-errors', "Payment system not initialized. Please refresh and try again.");
        currentDotNetRef.invokeMethodAsync('HandleStripeError', 'Stripe.js or Card Element not initialized.');
        return;
    }
    if (!currentClientSecret) {
        console.error("stripeInterop.js: Client Secret is missing. Cannot confirm card setup.");
        displayError('card-errors', "Configuration error: Client secret for payment processing is missing.");
        currentDotNetRef.invokeMethodAsync('HandleStripeError', 'Client Secret is missing for card setup confirmation.');
        return;
    }
    
    console.log("stripeInterop.js: Attempting to confirm card setup with Stripe...");
    const submitButton = document.getElementById('submit-button'); // Assuming 'submit-button' is always the ID
    if (submitButton) submitButton.disabled = true; // Disable button during processing

    try {
        const { setupIntent, error } = await stripe.confirmCardSetup(
            currentClientSecret, {
                payment_method: {
                    card: cardElement,
                    // billing_details: { name: 'Customer Name' } // Optional: Add billing details if needed
                }
            }
        );

        if (error) {
            // Error from confirmCardSetup (e.g., card declined, invalid CVC)
            console.error("stripeInterop.js: Stripe confirmCardSetup error:", error);
            let errorMessage = error.message;
            if (error.type === "validation_error") { // More specific handling if needed
                errorMessage = "Invalid card details: " + error.message;
            }
            displayError('card-errors', errorMessage);
            currentDotNetRef.invokeMethodAsync('HandleStripeError', errorMessage);
        } else {
            // Success
            console.log("stripeInterop.js: Stripe confirmCardSetup success. Status:", setupIntent.status);
            if (setupIntent.status === 'succeeded') {
                console.log("stripeInterop.js: SetupIntent Succeeded. PaymentMethod ID:", setupIntent.payment_method);
                currentDotNetRef.invokeMethodAsync('HandleStripeSuccess', setupIntent.payment_method);
            } else {
                // Handle other statuses if necessary, e.g., 'requires_action'
                const nonSucceededStatusMsg = `Card setup status: ${setupIntent.status}. Further action may be required.`;
                console.warn("stripeInterop.js: " + nonSucceededStatusMsg);
                displayError('card-errors', nonSucceededStatusMsg + " Please try again or contact support.");
                currentDotNetRef.invokeMethodAsync('HandleStripeError', nonSucceededStatusMsg);
            }
        }
    } catch (e) {
        // Unexpected exception during the API call
        console.error("stripeInterop.js: Exception during stripe.confirmCardSetup:", e);
        const exceptionMsg = "An unexpected error occurred while processing your card. Please try again.";
        displayError('card-errors', exceptionMsg);
        currentDotNetRef.invokeMethodAsync('HandleStripeError', exceptionMsg + " Details: " + e.message);
    } finally {
        if (submitButton) submitButton.disabled = false; // Re-enable button
    }
}

// Helper function to display errors in the designated card-errors div
function displayError(elementId, message) {
    const errorDiv = document.getElementById(elementId);
    if (errorDiv) {
        errorDiv.textContent = message;
    } else {
        // Fallback if the primary error display div is not found
        console.error("stripeInterop.js: Could not find error display element with ID:", elementId, "Message:", message);
        // As a last resort, alert, but generally not recommended for good UX.
        // alert("Error: " + message); 
    }
}

// Expose functions to global window object
window.stripeInterop = {
    initializeStripeElements,
    confirmCardSetupOnStripe
};

console.log("stripeInterop.js loaded and initialized.");
