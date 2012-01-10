Ext.setup({
    glossOnIcon: true,
    onReady: function () {
        var form = new Ext.form.FormPanel({
            items: [
                    {
                        xtype: 'fieldset',
                        id: 'loginFormSet',
                        title: '请登录',
                        items: [
                        {
                            xtype: 'textfield',
                            id: 'username',
                            name: 'username',
                            placeHolder: '用户名/设备号',
                            label: '帐号'
                        },
                        {
                            xtype: 'passwordfield',
                            id: 'password',
                            name: 'password',
                            label: '密码',
                            useClearIcon: true
                        },
                        {
                            xtype: 'button',
                            text: '登录',
                            ui: 'confirm',
                            style: 'margin:2%;',
                            handler: function () {
                                form.disable();
                                var u = Ext.util.Format.trim(form.getValues().username);
                                var p = Ext.util.Format.trim(form.getValues().password);
                                if (u == "") {
                                    Ext.Msg.alert('提示', '请输入帐号', Ext.emptyFn);
                                    form.enable();
                                    return;
                                }
                                if (p == "") {
                                    Ext.Msg.alert('提示', '请输入密码', Ext.emptyFn);
                                    form.enable();
                                    return;
                                }
                                var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "请稍候..." });
                                myMask.show();
                                Ext.Ajax.request({
                                    url: "handler/users.aspx?type=login",
                                    method: 'post',
                                    params: { username: u, password: p },
                                    success: function (response) {
                                        var obj = Ext.decode(response.responseText)[0];
                                        if (obj.success) {
                                            window.location = "index.aspx";
                                        } else {
                                            myMask.hide();
                                            Ext.Msg.alert('登录失败', obj.msg, Ext.emptyFn);
                                            form.enable();
                                        }
                                    },
                                    failure: function (response) {
                                        myMask.hide();
                                        Ext.Msg.alert('登录失败', '可能是网络问题,请重试', Ext.emptyFn);
                                        form.enable();
                                    },
                                    scope: this
                                });
                            }
                        }
                        ]
                    }
                ]
        });
        var panel = new Ext.Panel({
            fullscreen: true,
            items: [form, { html: '<div style="margin: 0 auto;width:240px;"><a href="exsunandroid.apk" target="_blank"><img alt="android" src="android.png" /></a></div><div style="margin: 0 auto;width:240px;"><img alt="iphone" src="iphone.png" /></div>'}],
            scope: this
        });
    }
});