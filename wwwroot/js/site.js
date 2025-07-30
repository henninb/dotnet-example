document.addEventListener('DOMContentLoaded', function() {
    const sourceTimezoneSelect = document.querySelector('select[name="SourceTimezone"]');
    const targetTimezoneSelect = document.querySelector('select[name="TargetTimezone"]');
    const sourceTimeInput = document.querySelector('input[name="SourceTime"]');
    const form = document.querySelector('form');
    const validationSummary = document.querySelector('[data-valmsg-summary]') || document.querySelector('.alert-danger');

    if (sourceTimeInput && !sourceTimeInput.value) {
        const now = new Date();
        const localIsoString = new Date(now.getTime() - now.getTimezoneOffset() * 60000).toISOString().slice(0, 16);
        sourceTimeInput.value = localIsoString;
    }

    function showValidationError(message) {
        if (validationSummary) {
            validationSummary.innerHTML = '<ul><li>' + message + '</li></ul>';
            validationSummary.style.display = 'block';
        }
    }

    function hideValidationError() {
        if (validationSummary) {
            validationSummary.style.display = 'none';
        }
    }

    if (form) {
        form.addEventListener('submit', function(e) {
            hideValidationError();
            
            if (!sourceTimezoneSelect.value) {
                e.preventDefault();
                showValidationError('Please select a source timezone.');
                return;
            }
            
            if (!targetTimezoneSelect.value) {
                e.preventDefault();
                showValidationError('Please select a target timezone.');
                return;
            }
            
            if (!sourceTimeInput.value) {
                e.preventDefault();
                showValidationError('Please enter a date and time.');
                return;
            }
        });
    }

    function updateCurrentTimeButton() {
        const currentTimeBtn = document.createElement('button');
        currentTimeBtn.type = 'button';
        currentTimeBtn.className = 'btn btn-outline-secondary btn-sm mt-1';
        currentTimeBtn.innerHTML = 'Use Current Time';
        currentTimeBtn.addEventListener('click', function() {
            const now = new Date();
            const localIsoString = new Date(now.getTime() - now.getTimezoneOffset() * 60000).toISOString().slice(0, 16);
            sourceTimeInput.value = localIsoString;
        });
        
        if (sourceTimeInput && sourceTimeInput.parentNode) {
            sourceTimeInput.parentNode.appendChild(currentTimeBtn);
        }
    }

    updateCurrentTimeButton();
});