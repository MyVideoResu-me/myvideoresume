function saveTextAsFile(textToWrite, fileNameToSaveAs) {
    var textFileAsBlob = new Blob([textToWrite], { type: 'text/plain' });
    var downloadLink = document.createElement("a");
    downloadLink.download = fileNameToSaveAs;
    downloadLink.innerHTML = "Download File";
    if (window.webkitURL != null) {
        // Chrome allows the link to be clicked
        // without actually adding it to the DOM.
        downloadLink.href = window.webkitURL.createObjectURL(textFileAsBlob);
    }
    else {
        // Firefox requires the link to be added to the DOM
        // before it can be clicked.
        downloadLink.href = window.URL.createObjectURL(textFileAsBlob);
        downloadLink.onclick = destroyClickedElement;
        downloadLink.style.display = "none";
        document.body.appendChild(downloadLink);
    }

    downloadLink.click();
}

function copyToClipboard(jsonText) {
    var text = "";
    try {
        if (jsonText) {
            text = jsonText;
        }
        else {
            var element = document.getElementById("resumeJson");
            if (element) {
                text = element.innerText;
            }
        }
        navigator.clipboard.writeText(text);
    }
    catch (err) {
        console.error('Failed to copy: ', err);
    }
}

function sendHeightToParent() {
    const height = document.documentElement.scrollHeight;
    window.parent.postMessage({ height: height });
}

// Call sendHeightToParent when the content loads and when it changes (e.g., on window resize)
window.addEventListener('load', sendHeightToParent);
window.addEventListener('resize', sendHeightToParent);

// Optionally, you can also send height updates periodically using setInterval
// setInterval(sendHeightToParent, 500); // Send height every 500ms

