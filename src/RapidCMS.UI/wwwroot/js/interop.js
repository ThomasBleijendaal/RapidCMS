window['RapidCMS'] = {
    navigateTo: function (uri) {
        console.log(uri);
        history.replaceState(null, /* ignored title */ '', uri);
    }
};