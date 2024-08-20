mergeInto(
    LibraryManager.library,
    {
        InitializeYandexSDK: function() {
            Initialize();  
        },
        
        Review: function () {
            _ysdk.feedback.canReview()
                .then(({value, reason}) => {
                    if (value) {
                        _ysdk.feedback.requestReview()
                            .then(({feedbackSent}) => {
                                console.log(feedbackSent);
                                SendCallback("OnReview");
                            })
                    } else {
                        console.log(reason)
                        SendCallback("OnReview");
                    }
                }).catch(error => {
                SendCallback("OnReview");
            })
        },

        SaveUserData: function (data) {
            let dateString = UTF8ToString(data);
            let parsedData = JSON.parse(dateString);
            _player.setData(parsedData);
        },

        GetUserData: function () {
            console.log("Request user data");
            _player.getData().then(data => {
                console.log("Convert received data");
                const rawData = JSON.stringify(data);
                console.log("On data received");
                console.log(rawData);
                SendDataCallback("OnUserDataReceived", rawData);
            });
        },

        SetLeaderboard: function (target, value) {
            _leaderboards.setLeaderboardScore(target, value);
        },

        GetLeaderboardEntries: function (target, quantityTop, quantityAround) {
            _leaderboards.getLeaderboardEntries(target, {
                quantityTop: quantityTop,
                includeUser: true,
                quantityAround: quantityAround
            })
                .then(res => {
                    console.log(res);
                    const rawData = JSON.stringify(res);
                    SendDataCallback("OnLeaderboardsReceived", rawData)
                });
        },

        GetLang: function () {
            const lang = _ysdk.environment.i18n.lang;
            const bufferSize = lengthBytesUTF8(lang) + 1;
            const buffer = _malloc(bufferSize);
            stringToUTF8(lang, buffer, bufferSize);
            return buffer;
        },

        ShowFullscreenAd: function () {
            _ysdk.adv.showFullscreenAdv({
                callbacks: {
                    onClose: function (wasShown) {
                        SendCallback("OnInterstitialShown");
                    },
                    onError: function (error) {
                        SendDataCallback("OnInterstitialShown", error);
                    }
                }
            })
        },

        ShowRewardedAd: function () {
            _ysdk.adv.showRewardedVideo({
                callbacks: {
                    onOpen: () => {
                        console.log('Video ad open.');
                    },
                    onRewarded: () => {
                        console.log('Rewarded!');
                        SendCallback("OnRewarded");
                    },
                    onClose: () => {
                        console.log('Video ad closed.');
                        SendCallback("OnRewardedClose");
                    },
                    onError: (e) => {
                        console.log('Error while open video ad:', e);
                        SendDataCallback("OnRewardedError", e);
                    }
                }
            })
        },

        Purchase: function (purchaseId) {
            _payments.purchase({id: purchaseId}).then(purchase => {
                SendDataCallback("OnPurchaseSuccess", purchaseId)
            }).catch(e => {
                SendDataCallback("OnPurchaseFailed", e)
            })
        },

        GetProducts: function () {
            _payments.getCatalog().then(products => {
                let productsJson = JSON.stringify(products);
                SendDataCallback("OnProductsReceived", productsJson)
            }).catch(e => {
                console.log('Error while receiving catalog ', e);
            });
        },

        GetPurchases: function () {
            _payments.getPurchases().then(purchases => {
                let purchasesJson = JSON.stringify(purchases);
                SendDataCallback("OnPurchasesReceived", purchasesJson)
            }).catch(e => {
                console.log('Error while receiving purchases ', e);
            });
        }
    });