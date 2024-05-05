window.downloadFile = async (filename, contentStreamRefrence) => {
    const arrayBuffer = await contentStreamRefrence.arrayBuffer();
    const blob = new Blob([arrayBuffer]);
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename ?? '';
    a.click();
    a.remove();
    URL.revokeObjectURL(url);
};