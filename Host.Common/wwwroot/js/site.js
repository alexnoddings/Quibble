window.quibbleInterop = {
    clipboard: {
        write: function (content) {
            navigator.clipboard.writeText(content).catch(function (err) { });
        }
    }
};