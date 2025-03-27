mergeInto(LibraryManager.library,
    {
        GetLanguageItch: function () {
            const urlParams = new URLSearchParams(window.location.search);
            const userId = urlParams.get('userId');
            console.log("User ID:", userId);
            return userId;
        }
    }
);