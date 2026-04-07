function showSuccess(message) {
    iziToast.show({
        title: 'Success',
        message: message,
        backgroundColor: '#0f1b2d',
        titleColor: '#ffffff',
        messageColor: '#c7d2e3',
        iconColor: '#e2b400',
        color: '#e2b400',
        progressBarColor: '#e2b400',
        borderColor: 'rgba(255,255,255,0.08)',
        boxShadow: '0 18px 40px rgba(0,0,0,0.35)'
    });
}

function showError(message) {
    iziToast.show({
        title: 'Error',
        message: message,
        backgroundColor: '#1a1620',
        titleColor: '#ffffff',
        messageColor: '#d7d2dc',
        iconColor: '#ff6b6b',
        color: '#ff6b6b',
        progressBarColor: '#ff6b6b',
        borderColor: 'rgba(255,255,255,0.08)',
        boxShadow: '0 18px 40px rgba(0,0,0,0.35)'
    });
}

function showInfo(message) {
    iziToast.show({
        title: 'Info',
        message: message,
        backgroundColor: '#101a2a',
        titleColor: '#ffffff',
        messageColor: '#c7d2e3',
        iconColor: '#7ab8ff',
        color: '#7ab8ff',
        progressBarColor: '#7ab8ff',
        borderColor: 'rgba(255,255,255,0.08)',
        boxShadow: '0 18px 40px rgba(0,0,0,0.35)'
    });
}