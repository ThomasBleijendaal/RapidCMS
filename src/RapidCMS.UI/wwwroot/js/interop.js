window['RapidCMS'] = {
    navigateTo: function (uri) {
        // TODO: push the navigation state here..
        history.pushState(null, /* ignored title */ '', uri);
    }
};
