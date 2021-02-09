window['RapidCMS'] = {
    navigateTo: function (uri) {
        history.replaceState(null, /* ignored title */ '', uri);
    }
};