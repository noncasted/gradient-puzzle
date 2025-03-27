mergeInto(LibraryManager.library,
    {
        GetUserIdFromUrl: function () {
            const urlParams = new URLSearchParams(window.location.search);
            const userId = urlParams.get('userId');
            console.log("User ID:", userId);
            return userId;
        }
    }
);