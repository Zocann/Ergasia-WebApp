window.addEventListener('DOMContentLoaded', () => {
    const formToast = document.getElementById('formToast');
    if (formToast) {
        const toast = new bootstrap.Toast(formToast);
        toast.show();
    }
});