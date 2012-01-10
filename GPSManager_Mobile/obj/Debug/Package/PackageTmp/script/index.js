Ext.ns('exsun', 'Ext.ux');
Ext.regModel('Device', {
    fields: [
                { name: 'text', type: 'string' },
                { name: 'phone', type: 'string' },
                { name: 'count', type: 'int' }
            ]
});
var deviceStore = new Ext.data.TreeStore({
    model: 'Device',
    root: {
        items: structure
    },
    proxy: {
        type: 'ajax',
        reader: {
            type: 'tree',
            root: 'items'
        }
    }
});
var map = new Ext.Map({
    mapOptions: {
        center: new google.maps.LatLng(_centerlat, _centerlon),
        zoom: 12        
    }
});
var marker = new google.maps.Marker();
var infowindow = new google.maps.InfoWindow();
google.maps.event.addListener(marker, 'mousedown', function () {
    infowindow.open(map.map, marker);
});

Ext.ux.UniversalUI = Ext.extend(Ext.Panel, {
    fullscreen: true,
    layout: 'card',
    items: [map],
    backText: '返回',
    useTitleAsBackText: true,
    initComponent: function () {
        this.navigationButton = new Ext.Button({
            hidden: Ext.is.Phone || Ext.Viewport.orientation == 'landscape',
            text: '车辆列表',
            handler: this.onNavButtonTap,
            scope: this
        });

        this.backButton = new Ext.Button({
            text: this.backText,
            ui: 'back',
            handler: this.onUiBack,
            hidden: true,
            scope: this
        });
        var btns = [this.navigationButton];
        if (Ext.is.Phone) {
            btns.unshift(this.backButton);
        }

        this.navigationBar = new Ext.Toolbar({
            ui: 'dark',
            dock: 'top',
            title: this.title,
            items: btns.concat(this.buttons || [])
        });

        this.navigationPanel = new Ext.NestedList({
            store: deviceStore,
            useToolbar: Ext.is.Phone ? false : true,
            updateTitleText: false,
            dock: 'left',
            hidden: !Ext.is.Phone && Ext.Viewport.orientation == 'portrait',
            toolbar: Ext.is.Phone ? this.navigationBar : null,
            listeners: {
                itemtap: this.onNavPanelItemTap,
                scope: this
            }, 
            getItemTextTpl: function (node) {
                return '<tpl if="leaf"></tpl>' +
                '<tpl if="!leaf"><span>{text}</span>' +
                '<span style="font-size:.7em;color:#999;margin-left:5px;font-weight:bold;">({count})</span></tpl>' +
                '<tpl if="leaf"><span>{text}</span><span style="font-size:.7em;color:#999;margin-left:5px;">{phone}</span></tpl>';
            }
        });

        this.navigationPanel.on('back', this.onNavBack, this);

        if (!Ext.is.Phone) {
            this.navigationPanel.setWidth(250);
        }

        this.dockedItems = this.dockedItems || [];
        this.dockedItems.unshift(this.navigationBar);

        if (!Ext.is.Phone && Ext.Viewport.orientation == 'landscape') {
            this.dockedItems.unshift(this.navigationPanel);
        }
        else if (Ext.is.Phone) {
            this.items = this.items || [];
            this.items.unshift(this.navigationPanel);
        }

        this.addEvents('navigate');

        Ext.ux.UniversalUI.superclass.initComponent.call(this);
    },

    toggleUiBackButton: function () {
        var navPnl = this.navigationPanel;
        if (Ext.is.Phone) {
            if (this.getActiveItem() === navPnl) {
                var currList = navPnl.getActiveItem(),
                    currIdx = navPnl.items.indexOf(currList),
                    recordNode = currList.recordNode,
                    title = navPnl.renderTitleText(recordNode),
                    parentNode = recordNode ? recordNode.parentNode : null,
                    backTxt = (parentNode && !parentNode.isRoot) ? navPnl.renderTitleText(parentNode) : this.title || '',
                    activeItem;
                if (currIdx <= 0) {
                    this.navigationBar.setTitle(this.title || '');
                    this.backButton.hide();
                } else {
                    this.navigationBar.setTitle(title);
                    if (this.useTitleAsBackText) {
                        this.backButton.setText(backTxt);
                    }
                    this.backButton.show();
                }
            } else {
                activeItem = navPnl.getActiveItem();
                recordNode = activeItem.recordNode;
                backTxt = (recordNode && !recordNode.isRoot) ? navPnl.renderTitleText(recordNode) : this.title || '';
                if (this.useTitleAsBackText) {
                    this.backButton.setText(backTxt);
                }
                this.backButton.show();
            }
            this.navigationBar.doLayout();
        }
    },
    onUiBack: function () {
        var navPnl = this.navigationPanel;
        if (this.getActiveItem() === navPnl) {
            navPnl.onBackTap();
        } else {
            this.setActiveItem(navPnl, {
                type: 'slide',
                reverse: true
            });
        }
        this.toggleUiBackButton();
        this.fireEvent('navigate', this, {});
    },
    onNavPanelItemTap: function (subList, subIdx, el, e) {
        var store = subList.getStore(),
            record = store.getAt(subIdx),
            recordNode = record.node,
            nestedList = this.navigationPanel,
            title = nestedList.renderTitleText(recordNode),
            phone;
        if (record) {
            phone = record.get('phone');
        }
        if (phone) {
            this.setActiveItem(map, 'slide');
        }
        if (Ext.Viewport.orientation == 'portrait' && !Ext.is.Phone && !recordNode.childNodes.length) {
            this.navigationPanel.hide();
        }
        if (title) {
            this.navigationBar.setTitle(title);
        }
        this.toggleUiBackButton();
        this.fireEvent('navigate', this, record);
    },
    onNavButtonTap: function () {
        this.navigationPanel.showBy(this.navigationButton, 'fade');
    },
    layoutOrientation: function (orientation, w, h) {
        if (!Ext.is.Phone) {
            if (orientation == 'portrait') {
                this.navigationPanel.hide(false);
                this.removeDocked(this.navigationPanel, false);
                if (this.navigationPanel.rendered) {
                    this.navigationPanel.el.appendTo(document.body);
                }
                this.navigationPanel.setFloating(true);
                this.navigationPanel.setHeight(400);
                this.navigationButton.show(false);
            }
            else {
                this.navigationPanel.setFloating(false);
                this.navigationPanel.show(false);
                this.navigationButton.hide(false);
                this.insertDocked(0, this.navigationPanel);
            }
            this.navigationBar.doComponentLayout();
        }

        Ext.ux.UniversalUI.superclass.layoutOrientation.call(this, orientation, w, h);
    }
});

exsun.Main = {
    init: function () {
        this.refreshButton = new Ext.Button({
            ui: 'plain',
            iconCls: 'refresh',
            iconMask: true,
            hidden: true,
            handler: this.onRefreshButtonTap,
            scope: this
        });
        this.ui = new Ext.ux.UniversalUI({
            title: _cname,
            useTitleAsBackText: false,
            buttons: [{ xtype: 'spacer' }, this.refreshButton],
            listeners: {
                navigate: this.onNavigate,
                scope: this
            }
        });
    },
    onNavigate: function (ui, record) {
        if (record.data && record.data.phone) {
            if (record.data.phone == "logout") {
                window.location = "login.aspx";
                return;
            }
            marker.setMap(null);
            infowindow.close();
            Ext.Ajax.request({
                url: "handler/vehicles.aspx?p=" + record.data.phone,
                success: function (response) {
                    var obj = Ext.decode(response.responseText);
                    if (obj[0].lat != 0 && obj[0].lon != 0) {
                        var latlng = new google.maps.LatLng(obj[0].lat, obj[0].lon);
                        marker.id = record.data.phone;
                        marker.setTitle(record.data.text);
                        marker.setPosition(latlng);
                        marker.setMap(map.map);
                        var contentString = '<div style="font-size:100%;">设备名称:' +
                                record.data.text +
                                 '<br />设备号码:' +
                                 record.data.phone +
                                 '<br />详细位置:' +
                                 obj[0].place +
                                '</div><div style="color:#999;font-size:80%;text-align:right;">' +
                                obj[0].time +
                                '</div>';
                        infowindow.setContent(contentString);
                        infowindow.open(map.map, marker);
                        map.map.setCenter(latlng);
                        if (this.refreshButton.hidden) {
                            this.refreshButton.show();
                        }
                    } else {
                        this.refreshButton.hide();
                        var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "无法获取到该设备的位置" });
                        myMask.show();
                        setTimeout(function () { myMask.hide(); }, 1500);
                    }
                },
                failure: function (response) {
                    this.refreshButton.hide();
                    var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "暂时无法获取到该设备的位置" });
                    myMask.show();
                    setTimeout(function () { myMask.hide(); }, 1500);
                },
                scope: this
            });
        } else {
            this.refreshButton.hide();
        }
    },
    onRefreshButtonTap: function () {
        Ext.Ajax.request({
            url: "handler/vehicles.aspx?p=" + marker.id,
            success: function (response) {
                var obj = Ext.decode(response.responseText);
                if (obj[0].lat != 0 && obj[0].lon != 0) {
                    var latlng = new google.maps.LatLng(obj[0].lat, obj[0].lon);
                    marker.setPosition(latlng);
                    marker.setMap(map.map);
                    var contentString = '<div style="font-size:100%;">设备名称:' +
                                marker.getTitle() +
                                 '<br />设备号码:' +
                                 marker.id +
                                 '<br />详细位置:' +
                                 obj[0].place +
                                '</div><div style="color:#999;font-size:80%;text-align:right;">' +
                                obj[0].time +
                                '</div>';
                    infowindow.setContent(contentString);
                    infowindow.open(map.map, marker);
                    map.map.setCenter(latlng);
                } else {
                    var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "无法获取到该设备的位置" });
                    myMask.show();
                    setTimeout(function () { myMask.hide(); }, 1500);
                }
            },
            failure: function (response) {
                var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "暂时无法获取到该设备的位置" });
                myMask.show();
                setTimeout(function () { myMask.hide(); }, 1500);
            },
            scope: this
        });
    }
};

Ext.setup({
    glossOnIcon: true,
    onReady: function () {
        exsun.Main.init();
    }
});