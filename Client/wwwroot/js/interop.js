window.blazorInterop = {
    /**
     * Sets the location of the app. Alternative to NavigationManager.NavigateTo which doesn't add to the history.
     * Useful when redirecting so that going back doesn't immediately go forward again.
     * The router and NavigationManager will still fire properly.
     * @param {any} newUrl The url to go to.
     */
    setLocation: function (newUrl) {
        history.replaceState(null, "", newUrl);
    }
};
