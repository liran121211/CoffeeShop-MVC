function checkPasswords() {
    // Find the validation image div
    var validationElement = document.getElementById('passwordsValidation');
    // Get the form values
    var pwd = document.forms["register_form"]["Password"].value;
    var pwd_cfm = document.forms["register_form"]["ConfirmPassword"].value;
    // Reset the validation element styles
    validationElement.style.display = 'none';
    validationElement.className = 'validation-image';
    // Check if name2 isn't null or undefined or empty
    if (pwd_cfm) {
        // Show the validation element
        validationElement.style.display = 'inline-block';
        // Choose which class to add to the element
        validationElement.className += 
            (pwd == pwd_cfm ? ' validation-success' : ' validation-error');
    }
}