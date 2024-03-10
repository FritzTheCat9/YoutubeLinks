window.downloadFile = (filename, data) => {
    const blob = new Blob([data]);
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');

    a.style.display = 'none';
    a.href = url;
    a.download = filename ?? '';
    document.body.appendChild(a);
    a.click();
    window.URL.revokeObjectURL(url);
};