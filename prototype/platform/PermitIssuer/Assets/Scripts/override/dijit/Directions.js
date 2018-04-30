// COPYRIGHT © 2017 Esri
//
// All rights reserved under the copyright laws of the United States
// and applicable international laws, treaties, and conventions.
//
// This material is licensed for use under the Esri Master License
// Agreement (MLA), and is bound by the terms of that agreement.
// You may redistribute and use this code without modification,
// provided you adhere to the terms of the MLA and include this
// copyright notice.
//
// See use restrictions at http://www.esri.com/legal/pdfs/mla_e204_e300/english
//
// For additional information, contact:
// Environmental Systems Research Institute, Inc.
// Attn: Contracts and Legal Services Department
// 380 New York Street
// Redlands, California, USA 92373
// USA
//
// email: contracts@esri.com
//
// See http://js.arcgis.com/3.23/esri/copyright.txt for details.

define(["require", 
		"dojo/_base/declare",
		"dojo/_base/lang",
		"dojo/_base/kernel",
		"dojo/_base/array",
		"dojo/_base/Color",
		"dijit/a11yclick",
		"dijit/_TemplatedMixin",
		"dijit/form/Select",
		"dijit/form/ValidationTextBox", 
		"dijit/form/DateTextBox", 
		"dijit/form/TimeTextBox", 
		"dojo/store/Memory", 
		"dojo/data/ObjectStore", 
		"dojo/keys", 
		"dojo/has", 
		"dojo/on", 
		"dojo/mouse", 
		"dojo/dom", 
		"dojo/dom-geometry", 
		"dojo/dom-style", 
		"dojo/dom-class", 
		"dojo/dom-attr", "dojo/query", "dojo/number", "dojo/i18n!esri/nls/jsapi", "dojo/text!esri/dijit/templates/Directions.html", 
		"esri/dijit/Search",
		"dojo/dom-construct", "dojo/promise/all", "dojo/Deferred", "dojo/dnd/Source", "dojo/json", 
		"esri/kernel", "esri/urlUtils", "esri/graphic", "esri/units", "esri/TimeExtent", "esri/InfoTemplate", "esri/SpatialReference", "esri/layers/ArcGISDynamicMapServiceLayer", "esri/layers/GraphicsLayer", "esri/geometry/webMercatorUtils", "esri/geometry/geodesicUtils", "esri/arcgis/utils", "esri/geometry/Point", "esri/geometry/Extent", "esri/geometry/Polyline", "esri/geometry/mathUtils", "esri/symbols/SimpleMarkerSymbol", "esri/symbols/PictureMarkerSymbol", "esri/symbols/CartographicLineSymbol", "esri/symbols/TextSymbol", "esri/renderers/UniqueValueRenderer", "esri/symbols/Font", "esri/dijit/_EventedWidget", "esri/tasks/FeatureSet", "scripts/override/tasks/RouteTask", "esri/tasks/RouteParameters", "esri/tasks/GeometryService", "esri/tasks/DistanceParameters", "esri/tasks/PrintTask", "esri/tasks/PrintParameters", "esri/tasks/PrintTemplate", "esri/toolbars/edit", "esri/toolbars/draw", "esri/config", "esri/tasks/ProjectParameters", "dojo/uacss"], function (t, e, s, i, r, o, n, a, h, c, u, l, d, p, m, _, g, f, v, y, S, T, w, b, M, C, D, L, N, A, P, x, I, R, B, E, O, k, W, U, G, j, F, H, q, Z, V, z, K, J, Q, Y, $, X, tt, et, st, it, rt, ot, nt, at, ht, ct, ut, lt, dt, pt) {
	var mt = B.getProtocolForWebResource(),
	_t = e("esri.dijit.Directions", [et, a], {
			templateString: D,
			mapClickActive: !1,
			barrierToolActive: !1,
			_eventMap: {
				activate: !0,
				deactivate: !1,
				load: !0,
				"directions-start": !0,
				"directions-finish": ["result"],
				"directions-clear": !0,
				"segment-select": ["graphic"],
				"segment-highlight": ["graphic"],
				error: ["error"],
				"stops-update": ["stops"],
				"route-item-created": !0,
				"route-item-updated": !0,
				"feature-collection-created": !0
			},
			_emptyStop: {
				name: ""
			},
			constructor: function (e, i) {
				if (!e.map)
					throw Error('Required "map" parameter is missing. Cannot instantiate Directions Widget.');
				if (!i)
					throw Error('Required "srcNodeRef" parameter is missing. Cannot instantiate Directions Widget.');
				this._i18n = C,
				this._css = {
					widgetContainerClass: "esriDirectionsContainer",
					searchSourceContainerClass: "esriSearchSourceContainer",
					stopsContainerClass: "esriStopsContainer",
					stopsTableContainerClass: "esriStopsTableContainer",
					stopsTableCoverClass: "esriStopsTableCover",
					reverseStopsClass: "esriStopsReverse",
					addStopsClass: "esriStopsAdd",
					stopsClass: "esriStops",
					stopsRemovableClass: "esriStopsRemovable",
					stopsButtonContainerClass: "esriStopsButtons",
					stopsOptionsButtonClass: "esriStopsOptionsButton",
					stopsAddDestinationClass: "esriStopsAddDestination",
					stopsAddDestinationBtnClass: "esriStopsAddDestinationBtn",
					stopsGetDirectionsContainerClass: "esriStopsGetDirectionsContainer",
					stopsGetDirectionsClass: "esriStopsGetDirections",
					stopsClearDirectionsClass: "esriStopsClearDirections",
					stopsInnerGeocoderClass: "esriInnerGeocoder",
					stopsOptionsOptionsEnabledClass: "esriStopsOptionsEnabled",
					stopsOptionsMenuClass: "esriStopsOptionsMenu",
					stopsFindOptimalOrderClass: "esriFindOptimalOrderOption",
					stopsUseTrafficClass: "esriUseTrafficOption",
					stopsReturnToStartClass: "esriReturnToStartOption",
					stopsOptionsCheckboxesClass: "esriOptionsCheckboxes",
					stopsOptionsToggleContainerClass: "esriOptionsToggleContainer",
					stopsOptionsUnitsContainerClass: "esriOptionsUnitsContainer",
					stopsOptionsUnitsMiClass: "esriOptionsUnitsMi",
					stopsOptionsUnitsKmClass: "esriOptionsUnitsKm",
					stopsOptionsImpedanceContainerClass: "esriOptionsImpedanceContainer",
					stopsOptionsImpedanceTimeClass: "esriOptionsImpedanceTime",
					stopsOptionsImpedanceDistanceClass: "esriOptionsImpedanceDistance",
					stopClass: "esriStop",
					stopOriginClass: "esriStopOrigin",
					stopDestinationClass: "esriStopDestination",
					stopUnreachedFirstOrLastClass: "esriStopUnreachedFirstOrLast",
					stopUnreachedClass: "esriStopUnreached",
					esriStopGeocoderColumnClass: "esriStopGeocoderColumn",
					esriStopReverseColumnClass: "esriStopReverseColumn",
					stopDnDHandleClass: "esriStopDnDHandle",
					stopDnDHandleClassHidden: "esriStopDnDHandleHidden",
					stopIconColumnClass: "esriStopIconColumn",
					stopIconClass: "esriStopIcon",
					stopIconRemoveColumnClass: "esriStopIconRemoveColumn",
					stopIconRemoveClass: "esriStopIconRemove",
					stopIconRemoveClassHidden: "esriStopIconRemoveHidden",
					resultsContainerClass: "esriResultsContainer",
					resultsLoadingClass: "esriResultsLoading",
					resultsPrintClass: "esriResultsPrint",
					resultsSaveClass: "esriResultsSave",
					resultsSummaryClass: "esriResultsSummary",
					routesContainerClass: "esriRoutesContainer",
					routesClass: "esriRoutes",
					routesErrorClass: "esriRoutesError",
					routesInfoClass: "esriRoutesInfo",
					routeClass: "esriRoute",
					routeTextColumnClass: "esriRouteTextColumn",
					routeTextClass: "esriRouteText",
					routeLengthClass: "esriRouteLength",
					routeOriginClass: "esriDMTStopOrigin",
					routeDestinationClass: "esriDMTStopDestination",
					routeInfoClass: "esriRouteInfo",
					routeIconColumnClass: "esriRouteIconColumn",
					routeIconClass: "esriRouteIcon",
					infoWindowRouteClass: "esriInfoWindowRoute",
					routeZoomClass: "esriRouteZoom",
					esriPrintPageClass: "esriPrintPage",
					esriPrintBarClass: "esriPrintBar",
					esriPrintButtonClass: "esriPrintButton",
					esriCloseButtonClass: "esriCloseButton",
					esriPrintMainClass: "esriPrintMain",
					esriPrintHeaderClass: "esriPrintHeader",
					esriPrintLogoClass: "esriPrintLogo",
					esriPrintMapClass: "esriPrintMap",
					esriPrintNameClass: "esriPrintName",
					esriPrintNotesClass: "esriPrintNotes",
					esriPrintLengthClass: "esriPrintLength",
					esriPrintDirectionsClass: "esriPrintDirections",
					esriPrintFooterClass: "esriPrintFooter",
					esriPrintStopLabelClass: "esriPrintStopLabel",
					clearClass: "esriClear",
					dndDragBodyClass: "esriDndDragDirection",
					stopsButtonClass: "esriDirectionsButton",
					stopsButtonTabClass: "esriDirectionsTabButton",
					stopsButtonTabLastClass: "esriDirectionsTabLastButton",
					stopsPressedButtonClass: "esriDirectionsPressedButton",
					linkButtonClass: "esriLinkButton",
					activateButtonClass: "esriActivateButton",
					lineBarrierButtonClass: "esriLineBarrierButton",
					travelModesContainerClass: "esriTravelModesContainer"
				},
				this.options = {
					map: null,
					autoSolve: !0,
					minStops: 2,
					maxStops: 20,
					theme: "simpleDirections",
					alphabet: "1234567890",
					directions: null,
					returnToStart: !1,
					optimalRoute: !1,
					routeTaskUrl: mt + "//route.arcgis.com/arcgis/rest/services/World/Route/NAServer/Route_World",
					printTaskUrl: mt + "//utility.arcgisonline.com/arcgis/rest/services/Utilities/PrintingTools/GPServer/Export%20Web%20Map%20Task",
					geometryTaskUrl: mt + "//utility.arcgisonline.com/arcgis/rest/services/Geometry/GeometryServer",
					routeParams: {},
					stops: ["", ""],
					searchOptions: {},
					stopsInfoTemplate: new W(C.widgets.directions.stop, "${address}${error}"),
					waypointInfoTemplate: new W(C.widgets.directions.maneuver, C.widgets.directions.waypoint),
					segmentInfoTemplate: new W(C.widgets.directions.maneuver, '<div class="${maneuverType}"><div class="' + this._css.routeIconClass + " " + this._css.infoWindowRouteClass + '"><strong>${step}.</strong> ${formattedText}</div></div>'),
					textSymbolFont: new tt("11px", tt.STYLE_NORMAL, tt.VARIANT_NORMAL, tt.WEIGHT_NORMAL, "Arial, Helvetica, sans-serif"),
					textSymbolColor: new o([255, 255, 255]),
					textSymbolOffset: {
						x: 0,
						y: 10.875
					},
					fromSymbol: new Q({
						url: t.toUrl("esri/dijit/images/Directions/greenPoint.png"),
						height: 21.75,
						width: 15.75,
						type: "esriPMS"
					}).setOffset(0, 10.875),
					fromSymbolDrag: new Q({
						url: t.toUrl("esri/dijit/images/Directions/greenPointMove.png"),
						height: 21.75,
						width: 15.75,
						type: "esriPMS"
					}).setOffset(0, 10.875),
					stopSymbol: new Q({
						url: t.toUrl("esri/dijit/images/Directions/bluePoint.png"),
						height: 21.75,
						width: 15.75,
						type: "esriPMS"
					}).setOffset(0, 10.875),
					stopSymbolDrag: new Q({
						url: t.toUrl("esri/dijit/images/Directions/bluePointMove.png"),
						height: 21.75,
						width: 15.75,
						type: "esriPMS"
					}).setOffset(0, 10.875),
					toSymbol: new Q({
						url: t.toUrl("esri/dijit/images/Directions/redPoint.png"),
						height: 21.75,
						width: 15.75,
						type: "esriPMS"
					}).setOffset(0, 10.875),
					toSymbolDrag: new Q({
						url: t.toUrl("esri/dijit/images/Directions/redPointMove.png"),
						height: 21.75,
						width: 15.75,
						type: "esriPMS"
					}).setOffset(0, 10.875),
					unreachedSymbol: new Q({
						url: t.toUrl("esri/dijit/images/Directions/grayPoint.png"),
						height: 21.75,
						width: 15.75,
						type: "esriPMS"
					}).setOffset(0, 10.875),
					unreachedSymbolDrag: new Q({
						url: t.toUrl("esri/dijit/images/Directions/grayPointMove.png"),
						height: 21.75,
						width: 15.75,
						type: "esriPMS"
					}).setOffset(0, 10.875),
					waypointSymbol: new J({
						color: [255, 255, 255, 255],
						size: 10,
						type: "esriSMS",
						style: "esriSMSCircle",
						outline: {
							color: [20, 89, 127, 255],
							width: 2.5,
							type: "esriSLS",
							style: "esriSLSSolid"
						}
					}),
					maneuverSymbol: new J({
						color: [255, 255, 255, 255],
						size: 4,
						type: "esriSMS",
						style: "esriSMSCircle",
						outline: {
							color: [30, 99, 137, 255],
							width: 1,
							type: "esriSLS",
							style: "esriSLSSolid"
						}
					}),
					routeSymbol: (new Y).setColor(new o([20, 89, 127, .75])).setWidth(10).setCap(Y.CAP_ROUND).setJoin(Y.JOIN_ROUND),
					segmentSymbol: (new Y).setColor(new o([255, 255, 255, 1])).setWidth(6).setCap(Y.CAP_ROUND).setJoin(Y.JOIN_ROUND),
					barrierRenderer: new X({
						type: "uniqueValue",
						field1: "BarrierType",
						defaultSymbol: {
							type: "esriPMS",
							imageData: "PHN2ZyB3aWR0aD0iMjIyIiBoZWlnaHQ9IjIyMiIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj4KPGc+CjxlbGxpcHNlIGZpbGw9IiNmZjAwMDAiIHN0cm9rZT0iIzAwMCIgc3Ryb2tlLXdpZHRoPSIyMCIgY3g9IjExMiIgY3k9IjExMSIgaWQ9InN2Z181IiByeD0iMTAwIiByeT0iMTAwIi8+CjxlbGxpcHNlIGZpbGw9IiNmZjAwMDAiIHN0cm9rZT0iI2ZmZmZmZiIgc3Ryb2tlLXdpZHRoPSIyMCIgY3g9IjExMiIgY3k9IjExMSIgaWQ9InN2Z182IiByeD0iOTUiIHJ5PSI5NSIvPgo8cmVjdCBmaWxsPSIjZmYwMDAwIiBzdHJva2Utd2lkdGg9IjIwIiB4PSI2NC41IiB5PSIxMDIiIHdpZHRoPSI5NSIgaGVpZ2h0PSIxOCIgaWQ9InN2Z183IiBzdHJva2U9IiNmZmZmZmYiLz4KPC9nPgo8L3N2Zz4=",
							contentType: "image/svg+xml",
							width: 18,
							height: 18
						},
						uniqueValueInfos: [{
								value: "2",
								symbol: {
									type: "esriPMS",
									imageData: "PHN2ZyB3aWR0aD0iMjg2IiBoZWlnaHQ9IjI1NiIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj4KPGc+CjxwYXRoIGZpbGw9IiNmZmYiIHN0cm9rZS13aWR0aD0iMTAiIGQ9Im04Ljc0OTk5MiwyNTEuNzQ5OTk3bDEzNC45OTk5OTgsLTI0MS45OTk5OThsMTM0Ljk5OTk5OCwyNDEuOTk5OTk4bC0yNjkuOTk5OTk1LDBsLTAuMDAwMDAxLDB6IiBpZD0ic3ZnXzEiIHN0cm9rZT0iIzAwMCIvPgo8cGF0aCBzdHJva2U9IiNmZmYiIGZpbGw9IiNmZjAwMDAiIHN0cm9rZS13aWR0aD0iMTUiIGQ9Im0yMy41MDAwMDQsMjQyLjUwMDAwNWwxMTkuOTk5OTkyLC0yMTUuOTk5OTk5bDExOS45OTk5OTIsMjE1Ljk5OTk5OWwtMjM5Ljk5OTk4MywwbC0wLjAwMDAwMSwweiIgaWQ9InN2Z18yIi8+Cjx0ZXh0IGZvbnQtd2VpZ2h0PSJib2xkIiBmaWxsPSIjZmZmZmZmIiBzdHJva2U9IiNmZjAwMDAiIHN0cm9rZS13aWR0aD0iMCIgeD0iMTA5IiB5PSIyMjIuNzUiIGlkPSJzdmdfNSIgZm9udC1zaXplPSIxODAiIGZvbnQtZmFtaWx5PSJHZW9yZ2lhLCBUaW1lcywgJ1RpbWVzIE5ldyBSb21hbicsIHNlcmlmIiB0ZXh0LWFuY2hvcj0ic3RhcnQiIHhtbDpzcGFjZT0icHJlc2VydmUiPiE8L3RleHQ+CjwvZz4KPC9zdmc+",
									contentType: "image/svg+xml",
									width: 19,
									height: 18
								}
							}
						]
					}),
					polylineBarrierRenderer: new X({
						type: "uniqueValue",
						field1: "BarrierType",
						defaultSymbol: {
							color: [255, 0, 0, 184],
							width: 7.5,
							type: "esriSLS",
							style: "esriSLSSolid"
						},
						uniqueValueInfos: [{
								value: "1",
								symbol: {
									color: [255, 85, 0, 184],
									width: 7.5,
									type: "esriSLS",
									style: "esriSLSSolid"
								}
							}
						]
					}),
					polygonBarrierRenderer: new X({
						type: "uniqueValue",
						field1: "BarrierType",
						defaultSymbol: {
							color: [255, 0, 0, 156],
							outline: {
								color: [255, 0, 0, 153],
								width: 2.4,
								type: "esriSLS",
								style: "esriSLSSolid"
							},
							type: "esriSFS",
							style: "esriSFSSolid"
						},
						uniqueValueInfos: [{
								value: "1",
								symbol: {
									color: [255, 170, 0, 156],
									outline: {
										color: [255, 0, 0, 153],
										width: 7.5,
										type: "esriSLS",
										style: "esriSLSSolid"
									},
									type: "esriSFS",
									style: "esriSFSSolid"
								}
							}
						]
					}),
					printPage: "",
					printTemplate: "",
					focusOnNewStop: !0,
					dragging: !0,
					canModifyStops: !0,
					canModifyWaypoints: !0,
					directionsLengthUnits: null,
					directionsLanguage: null,
					traffic: !1,
					trafficLayer: null,
					showPrintPage: !0,
					showSaveButton: !1,
					showSegmentPopup: !1,
					showSegmentHighlight: !0,
					showReverseStopsButton: !0,
					showReturnToStartOption: !0,
					showOptimalRouteOption: !0,
					showTravelModesOption: !0,
					showMilesKilometersOption: !0,
					showClearButton: !1,
					showActivateButton: !0,
					showBarriersButton: !0,
					loaded: !1,
					routeLayer: {
						itemId: null,
						title: null,
						isItemOwner: !0,
						ownerFolder: null
					},
					startTime: "now"
				};
				var r = {
					_waypointName: "DWWP",
					_userDefinedStopName: "UserDefinedStopName",
					_solveInProgress: !1,
					_moveInProgress: !1,
					_stopSequence: 1e3
				};
				if (this.userOptions = e, this.defaults = s.mixin({}, this.options, e, r), (!this.defaults.minStops || this.defaults.minStops < 2) && (this.defaults.minStops = 2), this.defaults.minStops > 2 && this.defaults.stops && "," === this.defaults.stops.toString())
					for (var n = 2; n < this.defaults.minStops; n++)
						this.defaults.stops.splice(0, 0, "");
				this.domNode = i
			},
			postCreate: function () {
				this.inherited(arguments),
				this.own(g(this._activateButtonNode, n, s.hitch(this, function () {
							this.mapClickActive ? this.deactivate() : this.activate()
						}))),
				this.own(g(this._lineBarrierButtonNode, n, s.hitch(this, function () {
							this.barrierToolActive ? this.deactivateBarrierTool() : this.activateBarrierTool()
						}))),
				this.own(g(this._addDestinationNode, n, s.hitch(this, this._addStopButton))),
				this.own(g(this._optionsButtonNode, n, s.hitch(this, this._toggleOptionsMenu))),
				this.own(g(this._saveMenuButton, n, s.hitch(this, this._toggleSaveMenu))),
				this.own(g(this._saveButton, n, s.hitch(this, function () {
							this._saveButton._enabled && this._storeRouteUI()
						}))),
				this.own(g(this._saveAsButton, n, s.hitch(this, function () {
							s.mixin(this.routeLayer, {
								itemId: null,
								title: null,
								ownerFolder: null
							}),
							this._storeRouteUI()
						}))),
				this.own(g(this._findOptimalOrderNode, n, s.hitch(this, this._toggleCheckbox))),
				this.own(g(this._returnToStartNode, n, s.hitch(this, this._toggleCheckbox))),
				this.own(g(this._useTrafficNode, n, s.hitch(this, this._toggleCheckbox))),
				this.own(g(this._useMilesNode, n, s.hitch(this, this._toggleUnits))),
				this.own(g(this._useKilometersNode, n, s.hitch(this, this._toggleUnits))),
				this.own(g(this._getDirectionsButtonNode, n, s.hitch(this, this.getDirections))),
				this.own(g(this._clearDirectionsButtonNode, n, s.hitch(this, function () {
							this.clearDirections()
						})));
				var t = s.hitch(this, function () {
						clearTimeout(this._startTimeMenu._hideTimer),
						this._startTimeMenu._hideTimer = setTimeout(s.hitch(this, function () {
									S.set(this._startTimeMenu, "display", "none")
								}), 100)
					});
				this.own(g(this._startTimeButtonNode, n, s.hitch(this, function () {
							this._startTimeButtonNode.disabled || "block" === S.get(this._startTimeMenu, "display") ? S.set(this._startTimeMenu, "display", "none") : (S.set(this._startTimeMenu, "display", "block"), this["now" === this.startTime ? "_startTimeMenuLeaveNow" : "none" === this.startTime ? "_startTimeMenuNone" : "_startTimeMenuDepartAt"].focus())
						}))),
				this.own(g(this._startTimeMenuLeaveNow, "blur", t)),
				this.own(g(this._startTimeMenuDepartAt, "blur", t)),
				this.own(g(this._startTimeMenuNone, "blur", t)),
				this.own(g(this._startTimeMenuLeaveNow, "mousedown", s.hitch(this, function () {
							this.startTime = "now",
							this._updateStartTimeUI(),
							this._clearDisplayBeforeSolve()
						}))),
				this.own(g(this._startTimeMenuDepartAt, "mousedown", s.hitch(this, function () {
							this.startTime = (new Date).getTime(),
							this._updateStartTimeUI(),
							this._clearDisplayBeforeSolve()
						}))),
				this.own(g(this._startTimeMenuNone, "mousedown", s.hitch(this, function () {
							this.startTime = "none",
							this._updateStartTimeUI(),
							this._clearDisplayBeforeSolve()
						}))),
				this._symbolEventPaddingDirections = (new Y).setColor(new o([0, 255, 0, 0])).setWidth(20).setCap(Y.CAP_ROUND),
				this._stopLayer = new j({
						id: "directions_stopLayer_" + this.id,
						displayOnPan: !0
					}),
				this._routeLayer = new j({
						id: "directions_routeLayer_" + this.id,
						displayOnPan: !0
					}),
				this._waypointsEventLayer = new j({
						id: "directions_waypointsEventLayer_" + this.id,
						displayOnPan: !0
					}),
				this._barriersLayer = new j({
						id: "directions_barriersLayer_" + this.id,
						displayOnPan: !0
					}),
				this._polylineBarriersLayer = new j({
						id: "directions_polylineBarriersLayer_" + this.id,
						displayOnPan: !0
					}),
				this._polygonBarriersLayer = new j({
						id: "directions_polygonBarriersLayer_" + this.id,
						displayOnPan: !0
					}),
				this._barriersLayer.setRenderer(this.defaults.barrierRenderer),
				this._polylineBarriersLayer.setRenderer(this.defaults.polylineBarrierRenderer),
				this._polygonBarriersLayer.setRenderer(this.defaults.polygonBarrierRenderer),
				this.map && (this.map.addLayer(this._routeLayer), this.map.addLayer(this._polygonBarriersLayer), this.map.addLayer(this._polylineBarriersLayer), this.map.addLayer(this._barriersLayer), this.map.addLayer(this._stopLayer), this._externalTimeExtent = this.map.timeExtent),
				this._snappingManager = this.map.enableSnapping({
						layerInfos: [{
								layer: this._waypointsEventLayer,
								snapToVertex: !1,
								snapToPoint: !0,
								snapToEdge: !0
							}
						],
						tolerance: 15
					}),
				this._setWidgetProperties();
				var e = new E(null, this.waypointSymbol, {}),
				i = s.hitch(this, function () {
						this._moveInProgress || this._solveInProgress || !e._isShown() || (this.editToolbar.deactivate(), this._stopLayer.remove(e), clearTimeout(e._solveTimeout), e._solveTimeout = null, e.attributes.isWaypoint = !0, e._isStopIcon && this.stopGraphics[e._index] && (this._stopLayer.add(this.stopGraphics[e._index]), this._stopLayer.add(this.textGraphics[e._index]))),
						e._showTooltip()
					});
				this._handle = s.mixin(e, {
						_isHandle: !0,
						_tooltip: N.create("div", {
							className: this.theme + " esriDirectionsRouteTooltip",
							onmouseover: i
						}, this.map.container),
						_showTooltip: s.hitch(this, function (t) {
							e._tooltip.style.display = !t || t instanceof MouseEvent ? "none" : "inline",
							t && (t = "string" == typeof t ? t : "<table class='esriRoutesTooltip'>" + this._renderDirectionsItemTR(t) + "</table>", e._tooltip.innerHTML !== t && (e._tooltip.innerHTML = t))
						}),
						_isShown: s.hitch(this, function () {
							return r.indexOf(this._stopLayer.graphics, e) > -1
						}),
						_remove: i
					}),
				this._activate(this.mapClickActive)
			},
			startup: function () {
				return this.inherited(arguments),
				this._enqueue(this._init)
			},
			destroy: function () {
				this.deactivate(),
				this.map.removeLayer(this._barriersLayer),
				this.map.removeLayer(this._polylineBarriersLayer),
				this.map.removeLayer(this._polygonBarriersLayer),
				this.map.removeLayer(this._routeLayer),
				this.map.removeLayer(this._stopLayer),
				this.map.removeLayer(this._waypointsEventLayer),
				this._disconnectEvents(),
				N.empty(this.domNode),
				this.inherited(arguments)
			},
			activate: function () {
				return this._enqueue(function () {
					this.deactivateBarrierTool().then(s.hitch(this, function () {
							this.set("mapClickActive", !0)
						}))
				})
			},
			deactivate: function () {
				return this._enqueue(function () {
					this.deactivateBarrierTool().then(s.hitch(this, function () {
							this.set("mapClickActive", !1)
						}))
				})
			},
			activateBarrierTool: function () {
				return this._enqueue(function () {
					this.set("mapClickActive", !1),
					this.set("barrierToolActive", !0)
				})
			},
			deactivateBarrierTool: function () {
				return this._enqueue(function () {
					this.set("mapClickActive", !1),
					this.set("barrierToolActive", !1)
				})
			},
			clearDirections: function () {
				return this._enqueue(function () {
					return this.clearMessages(),
					this._clearDirections()
				})
			},
			reset: function () {
				return this._enqueue(this._reset)
			},
			modifyStopSequence: function (t, e) {
				return this._enqueue(function () {
					return this._modifyStopSequence(t, e)
				})
			},
			onActivate: function () {},
			onDeactivate: function () {},
			onActivateBarrierTool: function () {},
			onDeactivateBarrierTool: function () {},
			onLoad: function () {
				this._enableButton(this._getDirectionsButtonNode)
			},
			onDirectionsStart: function () {
				this._clearDisplayBeforeSolve(),
				this.set("solving", !0),
				this._showLoadingSpinner(!0)
			},
			onDirectionsFinish: function () {
				this._showLoadingSpinner(!1),
				this.set("solving", !1)
			},
			onDirectionsClear: function () {},
			onSegmentSelect: function () {},
			onSegmentHighlight: function () {},
			onStopsUpdate: function () {},
			onRouteItemCreated: function () {},
			onRouteItemUpdated: function () {},
			onFeatureCollectionCreated: function () {},
			onError: function () {},
			removeStops: function () {
				return this.reset()
			},
			removeStop: function (t, e, s) {
				return this._enqueue(function () {
					return this.clearMessages(),
					this._removeStop(t, e, s)
				})
			},
			updateStops: function (t) {
				return this._enqueue(function () {
					return this._updateStops(t)
				})
			},
			addStops: function (t, e) {
				return this._enqueue(function () {
					return this._addStops(t, e)
				}).then(s.hitch(this, this.zoomToFullRoute))
			},
			addStop: function (t, e) {
				return this._enqueue(function () {
					return this._addStop(t, e)
				}, {
					_incrementalSolveStopRange: this._incrementalSolveStopRange
				})
			},
			updateStop: function (t, e, s) {
				return this._enqueue(function () {
					return this._updateStop(t, e, s)
				}, {
					_incrementalSolveStopRange: this._incrementalSolveStopRange
				})
			},
			setBarriers: function (t) {
				return this.routeParams.barriers = t,
				this._getDirections().then(s.hitch(this, function () {
						this.zoomToFullRoute()
					}))
			},
			setPolylineBarriers: function (t) {
				return this.routeParams.polylineBarriers = t,
				this._getDirections().then(s.hitch(this, function () {
						this.zoomToFullRoute()
					}))
			},
			setPolygonBarriers: function (t) {
				return this.routeParams.polygonBarriers = t,
				this._getDirections().then(s.hitch(this, function () {
						this.zoomToFullRoute()
					}))
			},
			clearMessages: function () {
				this.messages = [],
				this._msgNode && (this._msgNode.innerHTML = "")
			},
			getDirections: function (t) {
				return this._enqueue(function () {
					return this._getDirections().then(s.hitch(this, function () {
							t !== !0 && this.zoomToFullRoute()
						}))
				})
			},
			selectSegment: function (t) {
				if (!(!this.directions || !this.directions.features || 0 > t || t >= this.directions.features.length))
					for (var e = b("[data-segment]", this._resultsNode), s = 0; s < e.length; s++) {
						var i = parseInt(w.get(e[s], "data-segment"), 10);
						if (t === i && e[s] !== this._focusedDirectionsItem) {
							this._focusedDirectionsItem = e[s],
							this.centerAtSegmentStart(t),
							this.onSegmentSelect(this.directions.features[t]),
							e[s].focus();
							break
						}
					}
			},
			unhighlightSegment: function (t) {
				var e = this._segmentGraphics;
				if (e && (!this._focusedDirectionsItem || t)) {
					for (var s = 0; s < e.length; s++)
						this._routeLayer.remove(e[s]);
					this._segmentGraphics = []
				}
			},
			highlightSegment: function (t, e) {
				if (!(this._focusedDirectionsItem && !e || t >= this.directions.features.length)) {
					t = t || 0;
					var i = s.hitch(this, function (t) {
							var e = this.map.toMap({
									x: 0,
									y: 0
								});
							return this.map.toScreen(e.offset(t, 0)).x
						}),
					r = s.hitch(this, function (t) {
							for (var e = 0, s = 0; s < t.length; s++)
								for (var r = 1; r < t[s].length; r++) {
									var o = t[s][r - 1],
									n = t[s][r];
									e += i(Math.sqrt((o[0] - n[0]) * (o[0] - n[0]) + (o[1] - n[1]) * (o[1] - n[1])))
								}
							return e
						}),
					n = s.hitch(this, function (t) {
							var e = this.map.toMap({
									x: 0,
									y: 0
								});
							return this.map.toMap({
								x: t,
								y: 0
							}).x - e.x
						}),
					a = function (t, e, s) {
						e = Math.max(1, e);
						for (var r, o, n, a, h = s ? t[0].length - 1 : 0, c = 0, u = [[t[0][s ? h : 0]]]; s && h > 0 || !s && h < t[0].length - 1; ) {
							if (o = t[0][s ? h - 1 : h], n = t[0][s ? h : h + 1], r = i(Math.sqrt((o[0] - n[0]) * (o[0] - n[0]) + (o[1] - n[1]) * (o[1] - n[1])))) {
								if (!(e > c + r)) {
									a = (e - c) / r,
									s ? u[0].splice(0, 0, [n[0] - (n[0] - o[0]) * a, n[1] - (n[1] - o[1]) * a]) : u[0].push([o[0] + (n[0] - o[0]) * a, o[1] + (n[1] - o[1]) * a]);
									break
								}
								s ? u[0].splice(0, 0, o) : u[0].push(n),
								c += r
							}
							h += s ? -1 : 1
						}
						return c + r > 0 ? u : t
					},
					h = this.get("directions").features[t],
					c = new z(h.geometry),
					u = 50,
					l = 15,
					d = 22,
					p = 40 * Math.PI / 180;
					if (s.mixin(h.attributes, {
							_index: t
						}), t) {
						var m = a(this.get("directions").features[t - 1].geometry.paths, u / 2, !0),
						_ = "esriDMTStop" !== h.attributes.maneuverType ? a(c.paths, u / 2, !1) : m,
						g = _ !== m ? [m[0].concat(_[0])] : _,
						f = new z(_).getExtent();
						if (_[0].length > 1 && i(Math.max(f.getWidth(), f.getHeight())) >= l) {
							for (var v, y = Math.cos(p / 2) * l, S = Math.sin(p / 2) * l, T = _[0].length - 2, w = _[0][T + 1], b = 0; T >= 0 && !b; )
								v = _[0][T], b = i(Math.sqrt((v[0] - w[0]) * (v[0] - w[0]) + (v[1] - w[1]) * (v[1] - w[1]))), T--;
							if (d > b) {
								var M = b + (d - b) / 3,
								C = b + 2 * (d - b) / 3,
								D = [w[0] - M / b * (w[0] - v[0]), w[1] - M / b * (w[1] - v[1])],
								L = [v[0] + C / b * (w[0] - v[0]), v[1] + C / b * (w[1] - v[1])];
								v = D,
								w = L,
								b = d
							}
							var N,
							A,
							P = y / b,
							x = [w[0] - (w[0] - v[0]) * P, w[1] - (w[1] - v[1]) * P];
							v[1] !== w[1] ? (P = (w[0] - v[0]) / (w[1] - v[1]), N = n(S / Math.sqrt(1 + P * P)), A = P * N) : (N = 0, A = n(S)),
							Math.abs(N) === 1 / 0 || Math.abs(A) === 1 / 0 || isNaN(N) || isNaN(A) || (g[0].push(w), g[0].push([x[0] - N, x[1] + A]), g[0].push([x[0] + N, x[1] - A]), g[0].push(w))
						} else
							g = a(g, 2 * r(_), !0);
						c.paths = g
					}
					this.unhighlightSegment(this._segmentGraphics && this._segmentGraphics.length);
					var I = s.clone(this.routeSymbol).setWidth(this.segmentSymbol.width).setColor(new o([parseInt(.9 * this.segmentSymbol.color.r), parseInt(.9 * this.segmentSymbol.color.g), parseInt(.9 * this.segmentSymbol.color.b)]));
					this._segmentGraphics = [new E(h.geometry, I, h.attributes, this.get("segmentInfoTemplate")), new E(c, this.routeSymbol, h.attributes, this.get("segmentInfoTemplate")), new E(c, this.segmentSymbol, h.attributes, this.get("segmentInfoTemplate"))],
					this.get("showSegmentHighlight") && (this._routeLayer.add(this._segmentGraphics[0]), this._routeLayer.add(this._segmentGraphics[1]), this._routeLayer.add(this._segmentGraphics[2]));
					var R = s.hitch(this, function (t) {
							if (t > 0 && t < this.directions.features.length)
								for (var e = this.directions.features[t]._associatedFeaturesWithWaypoints, s = 0; s < e.length; s++)
									e[s]._associatedSnapFeature && e[s]._associatedSnapFeature.getDojoShape() && e[s]._associatedSnapFeature.getDojoShape().moveToFront()
						});
					R(t - 1),
					R(t),
					this.onSegmentHighlight(this.directions.features[t])
				}
			},
			zoomToSegment: function (t) {
				var e,
				i = new P;
				return this.directions && this.directions.features ? (e = Math.max(0, Math.min(this.directions.features.length - 1, t || 0)), this.map.setExtent(this.get("directions").features[e].geometry.getExtent(), !0).promise.always(s.hitch(this, function () {
							this.highlightSegment(e),
							i.resolve()
						}))) : i.reject(new Error("No directions.")),
				i.promise
			},
			centerAtSegmentStart: function (t) {
				var e,
				i = new P;
				if (this.directions && this.directions.features) {
					e = Math.max(0, Math.min(this.directions.features.length - 1, t || 0));
					var r = this.directions.features[e];
					this.map.centerAt(r.geometry.getPoint(0, 0)).promise.always(s.hitch(this, function () {
							this.highlightSegment(e, !0),
							this._showSegmentPopup(r, e),
							i.resolve()
						}))
				} else
					i.reject(new Error("No directions."));
				return i.promise
			},
			zoomToFullRoute: function () {
				var t = new P;
				return this.directions && this.directions.features ? (this._clearInfoWindow(), this.unhighlightSegment(), this.get("map").setExtent(this.get("directions").extent, !0).promise.always(t.resolve)) : t.resolve(),
				t.promise
			},
			setListIcons: function () {
				var t,
				e = this._dnd.getAllNodes();
				for (t = 0; t < e.length; t++) {
					var s = b("." + this._css.stopIconClass, e[t])[0];
					s && (s.innerHTML = this._getLetter(t)),
					T.remove(e[t], this._css.stopOriginClass + " " + this._css.stopDestinationClass + " " + this._css.stopUnreachedClass + " " + this._css.stopUnreachedFirstOrLastClass);
					var i = this._getStopSymbol(this.stops[t]);
					i === this.fromSymbol ? T.add(e[t], this._css.stopOriginClass) : i === this.toSymbol ? T.add(e[t], this._css.stopDestinationClass) : i === this.unreachedSymbol && T.add(e[t], this._css.stopUnreachedClass)
				}
				var r = b("[data-reverse-td]", this._dndNode)[0];
				N.destroy(r),
				this.get("showReverseStopsButton") && N.create("td", {
					"data-reverse-td": "true",
					rowspan: e.length,
					className: this._css.esriStopReverseColumnClass,
					innerHTML: '<div role="button" class="' + this._css.reverseStopsClass + '" data-reverse-stops="true" title="' + C.widgets.directions.reverseDirections + '"></div>',
					onmouseover: function (t) {
						t.stopPropagation()
					},
					onmouseout: function (t) {
						t.stopPropagation()
					}
				}, e[0])
			},
			addRouteSymbols: function () {
				if (this.stopGraphics.length) {
					this._moveLayersToFront();
					for (var t = 0; t < this.stopGraphics.length; t++)
						if (this.stopGraphics[t] && (!this._handle._isShown() || this._handle._isShown() && this._handle._index !== t)) {
							this._stopLayer.add(this.stopGraphics[t]);
							var e = this.stopGraphics[t].getDojoShape();
							e && e.moveToFront(),
							this._stopLayer.add(this.textGraphics[t]);
							var s = this.textGraphics[t].getDojoShape();
							s && s.moveToFront()
						}
					this._moveInProgress && !this._handle.attributes.isWaypoint && this._handle.getDojoShape() && this._handle.getDojoShape().moveToFront()
				}
			},
			createRouteSymbols: function () {
				this._clearStopGraphics();
				for (var t = this.stops, e = function (t) {
					var e = {};
					for (var s in t)
						t.hasOwnProperty(s)
							 && 0 === s.indexOf("Attr_") && (e[s] = t[s]);
						return e
					}, i = 0; i < t.length; i++) {
						var r = t[i];
						if (r && r.feature) {
							var o = r.feature.attributes,
							n = o ? o.Status : void 0,
							a = null;
							this._isStopAWaypoint(r) || (a = new $(this._getLetter(i), this.get("textSymbolFont"), this.get("textSymbolColor")), this.get("textSymbolOffset") && a.setOffset(this.get("textSymbolOffset").x, this.get("textSymbolOffset").y));
							var h = new E(r.feature.geometry, a, {
									address: r.name
								}, this.get("stopsInfoTemplate"));
							h._isStopLabel = !0,
							h._index = i;
							var c = new E(r.feature.geometry, this._getStopSymbol(r), s.mixin({
										address: r.name,
										Status: void 0 === n ? 0 : n,
										CurbApproach: o && o.CurbApproach ? o.CurbApproach : null,
										TimeWindowStart: o && o.TimeWindowStart ? o.TimeWindowStart : null,
										TimeWindowEnd: o && o.TimeWindowEnd ? o.TimeWindowEnd : null,
										isWaypoint: this._isStopAWaypoint(r)
									}, e(o)), this.get(this._isStopAWaypoint(r) ? "waypointInfoTemplate" : "stopsInfoTemplate"));
							c._isStopIcon = !0,
							c._index = i,
							this.stopGraphics[i] = c,
							this.textGraphics[i] = h
						}
					}
				this.set("stopGraphics", this.stopGraphics),
				this.set("textGraphics", this.textGraphics),
				this._showBarriers(),
				this.addRouteSymbols(),
				this.setListIcons()
			},
			setTravelMode: function (t) {
				return this._enqueue(function () {
					return this.clearMessages(),
					this._travelModeSelector.setValue(t),
					this._setTravelMode(t)
				})
			},
			getSupportedTravelModeNames: function () {
				var t = [],
				e = this.serviceDescription;
				if (e && e.supportedTravelModes && e.supportedTravelModes.length)
					for (var s = e.supportedTravelModes, i = 0; i < s.length; i++)
						t.push(s[i].name);
				return t
			},
			setDirectionsLengthUnits: function () {
				var t = 1 === arguments.length ? arguments[0] : this.get("directionsLengthUnits");
				return this._enqueue(function () {
					return this.clearMessages(),
					this._setDirectionsLengthUnits(t)
				})
			},
			setDirectionsLanguage: function () {
				var t = 1 === arguments.length ? arguments[0] : this.get("directionsLanguage");
				return this._enqueue(function () {
					return this.clearMessages(),
					this._setDirectionsLanguage(t)
				})
			},
			useMyCurrentLocation: function (t) {
				return this.clearMessages(),
				this._createLocateButton(this.geocoders[t], !0, !0)
			},
			loadRoute: function (t) {
				return this._enqueue(s.hitch(this, function () {
						return this._loadRoute(t)
					}))
			},
			_getStopsAttr: function () {
				return this.returnToStart && this._returnToStartStop ? this.stops.concat(this._returnToStartStop) : this.stops
			},
			_getTravelModeNameAttr: function () {
				return this.routeParams && this.routeParams.travelMode && this.routeParams.travelMode.name
			},
			_reset: function () {
				var t = this.mapClickActive || this.barrierToolActive;
				return this._clearBarriersGraphics(),
				this._setWidgetProperties(),
				this._init().then(s.hitch(this, function () {
						this.mapClickActive = !t,
						this.set("mapClickActive", t),
						this._searchSourceSelector && this._searchSourceSelector.setValue("all")
					}))
			},
			_activate: function () {
				var t = this.get("mapClickActive"),
				e = s.hitch(this, function (t) {
						for (var e = t ? [this.textGraphics, this.stopGraphics, this.displayedManeuverPointGraphics, this.displayedRouteGraphics] : [this.displayedRouteGraphics, this.displayedManeuverPointGraphics, this.stopGraphics, this.textGraphics], s = 0; s < e.length; s++)
							for (var i = e[s], r = 0; r < i.length; r++) {
								var o = i[r].getDojoShape();
								o && o[t ? "moveToBack" : "moveToFront"].call(o)
							}
					});
				this.drawToolbar && (this.barrierToolActive = !1, this.drawToolbar.deactivate(), T.remove(this._lineBarrierButtonNode, this._css.stopsPressedButtonClass)),
				this._addStopOnMapClickListener && this._addStopOnMapClickListener.remove(),
				t ? (this.map.activeDirectionsWidget && this.map.activeDirectionsWidget !== this && this.map.activeDirectionsWidget.deactivate(), this.map.activeDirectionsWidget = this, this._addStopOnMapClickListener = g(this.map, "click", s.hitch(this, function (t) {
								this.canModifyStops && !this._solveInProgress && (this.map.infoWindow.hide(), this.addStop(new E(t.mapPoint)))
							})), this.map.addLayer(this._waypointsEventLayer), this._moveLayersToFront(), T.add(this._activateButtonNode, this._css.stopsPressedButtonClass), this.onActivate()) : (this.map.removeLayer(this._waypointsEventLayer), T.remove(this._activateButtonNode, this._css.stopsPressedButtonClass), this.onDeactivate()),
				e(!t),
				this.emit("map-click-active", {
					mapClickActive: this.mapClickActive
				})
			},
			_activateBarrierTool: function () {
				var t = this.get("barrierToolActive");
				t ? (this.map.activeDirectionsWidget && this.map.activeDirectionsWidget !== this && this.map.activeDirectionsWidget.deactivate(), this.map.activeDirectionsWidget = this, this.drawToolbar.activate(lt.FREEHAND_POLYLINE), T.add(this._lineBarrierButtonNode, this._css.stopsPressedButtonClass), this.onActivateBarrierTool()) : (this.drawToolbar.deactivate(), T.remove(this._lineBarrierButtonNode, this._css.stopsPressedButtonClass), this.onDeactivateBarrierTool()),
				this.emit("barrier-tool-active", {
					barrierToolActive: this.barrierToolActive
				})
			},
			_moveLayersToFront: function () {
				var t = this.get("map"),
				e = t.graphicsLayerIds.length - 1;
				t.reorderLayer(this._routeLayer, e),
				t.reorderLayer(this._polygonBarriersLayer, e),
				t.reorderLayer(this._polylineBarriersLayer, e),
				t.reorderLayer(this._barriersLayer, e),
				t.reorderLayer(this._waypointsEventLayer, e),
				t.reorderLayer(this._stopLayer, e)
			},
			_destroyGeocoders: function () {
				for (; this.geocoders && this.geocoders.length; ) {
					if (this.geocoders[0])
						try {
							this.geocoders[0].destroy()
						} catch (t) {}
					this.geocoders.splice(0, 1)
				}
				this.geocoders = []
			},
			_disconnectEvents: function () {
				var t,
				e = this._clearDirections(!0);
				if (this._watchEvents && this._watchEvents.length)
					for (t = 0; t < this._watchEvents.length; t++)
						this._watchEvents[t].unwatch();
				if (this._onEvents && this._onEvents.length)
					for (t = 0; t < this._onEvents.length; t++)
						this._onEvents[t].remove();
				if (this._geocoderEvents)
					for (t = 0; t < this._geocoderEvents.length; t++)
						this._geocoderEvents[t].value.unwatch(), this._geocoderEvents[t].blur.remove(), this._geocoderEvents[t].select.remove(), this._geocoderEvents[t].suggest.remove();
				return this._onEvents = [],
				this._watchEvents = [],
				this._geocoderEvents = [],
				this._disconnectResults(),
				this._destroyGeocoders(),
				this._destroyGlobalGeocoder(),
				this._destroyDnD(),
				e
			},
			_getDirections: function () {
				var t = new P;
				return this._removeEmptyStops(),
				this._getStopCount() + this._getWaypointCount() > 1 && this.loaded ? (this.onDirectionsStart(), this.clearMessages(), this._dnd.sync(), this._sortGeocoders(), this._getCandidates(this.stops).then(s.hitch(this, function (e) {
							this.stops = e,
							this._setStops(),
							this._configureRoute().always(s.hitch(this, function (e) {
									t.resolve(e)
								}))
						}), s.hitch(this, function (e) {
							this.set("directions", null),
							this._clearRouteGraphics(),
							t.reject(e),
							this.onDirectionsFinish(e)
						}))) : this._clearDirections(!0).always(s.hitch(this, function () {
						this.createRouteSymbols(),
						t.resolve()
					})),
				t.promise
			},
			_clearDirections: function () {
				var t = new P;
				return this._handle && this._handle._remove(),
				this.get("routeParams") && this.get("routeParams").stops ? this.get("routeParams").stops.features.length ? (this.get("routeParams").stops.features = [], this.onDirectionsClear(), t.resolve()) : arguments.length ? t.resolve() : this._reset().then(t.resolve, t.reject) : t.resolve(),
				this.set("directions", null),
				this._clearDisplayBeforeSolve(),
				this._clearDisplayAfterSolve(),
				this._routeLayer.clear(),
				this._waypointsEventLayer.clear(),
				this._stopLayer.clear(),
				t.promise
			},
			_setTravelMode: function (t) {
				var e,
				s = new P,
				i = this.serviceDescription,
				r = function () {
					s.resolve(t)
				};
				if (i && i.supportedTravelModes && i.supportedTravelModes.length) {
					var o = i.supportedTravelModes,
					n = !1;
					for (e = 0; e < o.length; e++)
						if (o[e].name === t) {
							n = !0,
							!this.routeParams.travelMode || this.routeParams.travelMode && this.routeParams.travelMode.name !== t ? (this.routeParams.travelMode = o[e].impedanceAttributeName ? o[e] : o[e].itemId, this._checkStartTimeUIAvailability(), this._solveAndZoom().always(r)) : r(),
							this._travelModeSelector && this._travelModeSelector.domNode && (this._travelModeSelector.domNode.title = o[e].description);
							break
						}
					n || s.reject(t)
				} else
					s.reject(t);
				return s.promise
			},
			_setDirectionsLengthUnits: function (t) {
				this._clearDisplayBeforeSolve();
				var e = new P;
				return T.remove(this._useMilesNode, this._css.stopsPressedButtonClass),
				T.remove(this._useKilometersNode, this._css.stopsPressedButtonClass),
				t === O.KILOMETERS ? T.add(this._useKilometersNode, this._css.stopsPressedButtonClass) : t === O.MILES && T.add(this._useMilesNode, this._css.stopsPressedButtonClass),
				t === O.KILOMETERS || t === O.METERS || t === O.MILES || t === O.FEET || t === O.YARDS || t === O.NAUTICAL_MILES ? (this.directionsLengthUnits = t, e.resolve(t)) : e.reject(t),
				e.promise
			},
			_setDirectionsLanguage: function (t) {
				this._clearDisplayBeforeSolve();
				var e = new P;
				return t = this._setDirectionsLanguageByLocale(t),
				this._solveAndZoom().always(function () {
					e.resolve(t)
				}, e.reject),
				e.promise
			},
			_showLoadingSpinner: function (t) {
				void 0 === t && (t = this._requestQueueTail && !this._requestQueueTail.isFulfilled() || this._moveInProgress),
				t ? (T.add(this._widgetContainer, this._css.resultsLoadingClass), T.add(this._resultsNode, "esriRoutesContainerBusy")) : (T.remove(this._widgetContainer, this._css.resultsLoadingClass), T.remove(this._resultsNode, "esriRoutesContainerBusy"))
			},
			_enqueue: function (t, e) {
				var i = new P;
				return this._requestQueueTail || (this._requestQueueTail = new P, this._requestQueueTail.resolve()),
				this._requestQueueTail.promise.always(s.hitch(this, function () {
						try {
							s.mixin(this, {
								_incrementalSolveStopRange: null
							}, e);
							var r = t.call(this);
							r && "object" == typeof r && r.hasOwnProperty("isFulfilled") ? r.then(s.hitch(this, function (t) {
									i.resolve(t),
									this._showLoadingSpinner()
								}), s.hitch(this, function (t) {
									i.reject(t),
									this._showLoadingSpinner()
								})) : (i.resolve(r), this._showLoadingSpinner())
						} catch (o) {
							i.reject(o),
							this._showLoadingSpinner()
						}
					})),
				this._requestQueueTail = i,
				this._showLoadingSpinner(),
				i.promise
			},
			_createDnD: function () {
				this._dnd = new x(this._dndNode, {
						skipForm: !0,
						withHandles: !0
					})
			},
			_destroyDnD: function () {
				N.empty(this._dndNode),
				this._dnd && (this._dnd.destroy(), this._dnd = null)
			},
			_createDepartAtControls: function () {
				if (this._departAtTime)
					this.map && this._restoreMapTimeExtent(), this._useTrafficItemNode.title = C.widgets.directions.trafficLabelLive, T.remove(this._departAtContainer, "departAtContainerVisible"), this.startTime = "now", this._updateStartTimeUI();
				else {
					var t = this,
					e = function () {
						this._keepDirections || t._clearDisplayBeforeSolve(),
						this._keepDirections = !1
					},
					i = s.hitch(this, function () {
							this.map && this.map.disableKeyboardNavigation()
						}),
					r = s.hitch(this, function () {
							this.map && this.map.enableKeyboardNavigation()
						});
					this._departAtTime = new l({
							required: !0,
							value: new Date,
							onChange: e,
							onFocus: i,
							onBlur: r
						}, this._departAtTimeContainer),
					this._departAtDate = new u({
							required: !0,
							value: new Date,
							onChange: e,
							onFocus: i,
							onBlur: r,
							constraints: {
								min: new Date(864e5)
							}
						}, this._departAtDateContainer)
				}
			},
			_setStartTime: function (t, e, s) {
				if (isNaN(s))
					this.startTime = "now" === s ? s : "none";
				else {
					var i = s instanceof Date ? s : new Date(s),
					r = this.directions && this.directions.features && this.directions.features[0] && this.directions.features[0].attributes,
					o = 60 * -i.getTimezoneOffset() * 1e3,
					n = r && r.arriveTimeUTC ? r.ETA - r.arriveTimeUTC : o,
					a = new Date(i - o + n);
					this._departAtTime._keepDirections = !t,
					this._departAtDate._keepDirections = !t,
					this._departAtTime.setValue(a),
					this._departAtDate.setValue(a)
				}
				this._updateStartTimeUI()
			},
			_checkStartTimeUIAvailability: function () {
				var t = this._getImpedanceAttribute(),
				e = t ? t.units : "",
				s = this._isTimeUnits(e),
				i = s || this.serviceDescription && this.serviceDescription.currentVersion >= 10.6;
				this._startTimeButtonNode.disabled = !i,
				T[i ? "remove" : "add"].apply(this, [this._startTimeButtonNodeContainer, "esriLinkButtonDisabled"]),
				T[i ? "remove" : "add"].apply(this, [this._startTimeDDLArrow, "esriDirectionsDDLArrowDisabled"]),
				i || this.set("startTime", "now")
			},
			_usingAGOL: function (t) {
				return t || (t = this.routeTaskUrl),
				t.search(/^(https?:)*\/\/*[^.]*\.arcgis\.com.*$/i) > -1
			},
			_usingRouteAGOL: function () {
				return this.get("routeTaskUrl").search(/^(https?:)*\/\/route*[^.]*\.arcgis\.com.*$/i) > -1
			},
			_setSearchOptions: function () {
				var t = {
					maxResults: 1,
					locationToAddressDistance: 100
				},
				e = {
					map: this.get("map"),
					autoNavigate: !1,
					enableInfoWindow: !1,
					enableHighlight: !1,
					enableSourcesMenu: !1
				};
				this.searchOptions = s.mixin(t, this.defaults.searchOptions, e)
			},
			_setDefaultUnits: function () {
				if (!this.get("directionsLengthUnits")) {
					var t = "EN-US" === i.locale.toUpperCase() ? O.MILES : O.KILOMETERS;
					this.defaults.directionsLengthUnits ? t = this.defaults.directionsLengthUnits : this.userOptions.routeParams && this.userOptions.routeParams.directionsLengthUnits && (t = this.userOptions.routeParams.directionsLengthUnits),
					this.set("directionsLengthUnits", t)
				}
				this._setDirectionsLengthUnits(this.directionsLengthUnits)
			},
			_setTrafficOptions: function () {
				this._usingRouteAGOL() && !this.trafficLayer && (this.trafficLayer = new G(mt + "//traffic.arcgis.com/arcgis/rest/services/World/Traffic/MapServer?token=" + this.routeTask._url.query.token, {
							opacity: .4
						})),
				this.trafficLayer && this.trafficLayer.url && this._usingAGOL(this.trafficLayer.url) && (this._trafficAvailabilityButton.style.display = "inline-block"),
				this.set("showTrafficOption", (this.defaults.showTrafficOption || !this.defaults.hasOwnProperty("showTrafficOption")) && !!this.trafficLayer),
				this._optionsMenu()
			},
			_updateCanModifyStops: function () {
				this.canModifyStops || this.canModifyWaypoints || this.set("mapClickActive", !1),
				arguments[1] || !this.canModifyStops || this.canModifyWaypoints || this.set("mapClickActive", !0),
				this._showAddDestination(),
				this._showMapClickActiveButton(),
				this._stopsTableCover.style.display = this.canModifyStops ? "none" : "inline"
			},
			_updateCanAddWaypoints: function () {
				this.canModifyStops || this.canModifyWaypoints || this.set("mapClickActive", !1),
				arguments[1] || this.canModifyStops || !this.canModifyWaypoints || this.set("mapClickActive", !0),
				this._showMapClickActiveButton(),
				this._handle._remove()
			},
			_updateStartTimeUI: function () {
				isNaN(this.startTime) ? (this._startTimeButtonLabel.innerHTML = "now" == this.startTime ? this._i18n.widgets.directions.leaveNow : this._i18n.widgets.directions.noStartTime, T.remove(this._startTimeButtonNodeContainer, "departAtButton"), T.remove(this._departAtContainer, "departAtContainerVisible")) : (T.add(this._startTimeButtonNodeContainer, "departAtButton"), T.add(this._departAtContainer, "departAtContainerVisible"), this._startTimeButtonLabel.innerHTML = this._i18n.widgets.directions.departAt)
			},
			_setWidgetProperties: function () {
				this._disconnectEvents(),
				this.set(this.defaults),
				this.routeLayer = s.clone(this.defaults.routeLayer),
				this._folderSelector && (this._outputLayer.setValue(""), this._outputLayer.set("disabled", !0), this._folderSelector.set("disabled", !0)),
				this.set("stops", []),
				this._updateCanModifyStops()
			},
			_updateStops: function (t) {
				var e = new P;
				return t ? this.get("loaded") ? this._reset().then(s.hitch(this, function () {
						this._addStops(t).then(e.resolve, e.reject)
					}), e.reject) : this._addStops(t).then(e.resolve, e.reject) : e.reject(),
				e.promise
			},
			_removeStop: function (t, e, i, r) {
				var o = new P,
				n = s.hitch(this, function (t) {
						this.stops.splice(t, 1);
						var e = this._dnd.getAllNodes()[t],
						s = this.get("geocoders");
						this._geocoderEvents[e.id] && (this._geocoderEvents[e.id].blur.remove(), this._geocoderEvents[e.id].select.remove(), this._geocoderEvents[e.id].suggest.remove(), this._geocoderEvents[e.id].value.unwatch()),
						s[t].destroy(),
						s.splice(t, 1),
						this.set("geocoders", s),
						N.destroy(e),
						this._dnd.sync(),
						this._stopsRemovable(),
						this._optionsMenu(),
						this._checkMaxStops(),
						this.setListIcons(),
						this._sortGeocoders()
					});
				(0 > t || t >= this.stops.length || void 0 === t) && (t = this.stops.length - 1);
				for (var a = t, h = !1, c = this.stopGraphics[a] && this._isStopAWaypoint(this.stops[a]) || r; !h; )
					n(a), c ? h = !0 : (a -= 0 >= a || a < this.stops.length && this._isStopAWaypoint(this.stops[a]) ? 0 : 1, h = !this._isStopAWaypoint(this.stops[a]));
				for (; this.stops.length - this._getWaypointCount() < this.minStops; )
					this._addStop();
				return this._clearStopsStatusAttr(),
				this._setStops(),
				this.createRouteSymbols(),
				i ? (this._clearDisplayBeforeSolve(), this._clearDisplayAfterSolve(), this.createRouteSymbols(), o.resolve()) : this._solveAndZoom(e).then(o.resolve, o.reject),
				o.promise
			},
			_removeTrafficLayer: function () {
				this.trafficLayer && this.map && this.map.removeLayer(this.trafficLayer),
				this._trafficLayerAdded = !1
			},
			_addStops: function (t, e) {
				var i = new P,
				r = [],
				o = this.autoSolve;
				this.autoSolve = !1,
				void 0 === e && (e = this._getStopCount());
				for (var n = 0; n < Math.min(t.length, this.maxStops - this._getStopCount()); n++) {
					var a = new P;
					this._addStop(t[n], e + n, !0).always(a.resolve),
					r.push(a)
				}
				return A(r).always(s.hitch(this, function () {
						this.autoSolve = o,
						this._getDirections().always(function () {
							i.resolve(t)
						})
					})),
				i.promise
			},
			_addStop: function (t, e, i) {
				var r = new P;
				return this._checkMaxStops(),
				this.maxStopsReached ? (this._showMessage(C.widgets.directions.error.maximumStops), r.reject(new Error(C.widgets.directions.error.maximumStops))) : (void 0 === t && void 0 === e && (e = this.stops.length), t instanceof E && t.attributes && t.attributes.isWaypoint && (!e || e === this.stops.length || this._getStopCount() < 2) && !i ? (this._showMessage(C.widgets.directions.error.waypointShouldBeInBetweenStops), r.reject(new Error(C.widgets.directions.error.waypointShouldBeInBetweenStops))) : this._getCandidate(t).then(s.hitch(this, function (t) {
							this._isStopAWaypoint(t) && t && t.feature && (t.feature.attributes = s.mixin({}, t.feature.attributes, {
										isWaypoint: !0,
										CurbApproach: 3
									})),
							this._insertStop(t, e),
							this.autoSolve && "" !== t.name ? this._getDirections().always(function () {
								r.resolve(t, e)
							}) : r.resolve(t, e)
						}), s.hitch(this, function (t) {
							r.reject(t)
						}))),
				r.promise
			},
			_removeEmptyStops: function () {
				for (var t = 0, e = this.stops.length - this._getWaypointCount() - this.minStops; t < this.stops.length && e > 0; )
					this.stops[t] && this.stops[t].name ? t++ : (this._removeStop(t, !0, !0, !0), e--, this._moveInProgress && this._handle._index >= t && this._handle._isStopIcon && this._handle._index--)
			},
			_setReverseGeocode: function (t, e, i) {
				if (t.feature.geometry && i > -1) {
					var r = {
						address: t.name
					};
					this.stopGraphics[i] && (s.mixin(this.stopGraphics[i].attributes, r), this.stopGraphics[i].setGeometry(e)),
					this.textGraphics[i] && (s.mixin(this.textGraphics[i].attributes, r), this.textGraphics[i].setGeometry(e)),
					this.set("stopGraphics", this.stopGraphics),
					this.set("textGraphics", this.textGraphics);
					var o = this.geocoders[i];
					return o && o.inputNode && (o.value = t.name, o.inputNode.value = t.name),
					s.mixin(t.feature.attributes, this.stops[i].feature.attributes),
					this.stops[i] = t,
					this.stops[i].feature.setGeometry(e),
					this._setStops(),
					this._enqueue(function () {
						return this._getDirections()
					})
				}
			},
			_insertStop: function (t, e) {
				var s,
				i;
				if (void 0 === e) {
					for (s = 0; s < this.geocoders.length; s++)
						if (!this.geocoders[s].get("value")) {
							i = this.geocoders[s];
							break
						}
				} else
					s = e, this.geocoders[s] && !this.geocoders[s].get("value") && (i = this.geocoders[s]);
				!i || void 0 !== e && e !== s || this._isStopAWaypoint(t) ? (void 0 === e && (e = this.geocoders.length), this.stops.splice(e, 0, t), this._createGeocoder(t, e)) : (this.stops[s] = t, i.set("value", t.name), i._stopReference = t),
				this._optionsMenu()
			},
			_createGeocoder: function (t, e) {
				var i = this._dnd.getAllNodes(),
				o = !1,
				n = !1,
				a = i.length;
				i[e] ? (n = i[e], o = !0) : (n = !1, o = !1);
				var h = s.hitch(this, function (t, e) {
						var s = e ? this._css.stopDnDHandleClass : this._css.stopDnDHandleClassHidden,
						i = e ? this._css.stopDnDHandleClassHidden : this._css.stopDnDHandleClass;
						T.replace(t.children[0], s, i),
						this.geocoders.length > 2 && (s = e ? this._css.stopIconRemoveClass : this._css.stopIconRemoveClassHidden, i = e ? this._css.stopIconRemoveClassHidden : this._css.stopIconRemoveClass, T.replace(t.children[3].children[0], s, i))
					}),
				c = N.create("tr", {
						className: this._css.stopClass,
						style: this._isStopAWaypoint(t) ? "display:none;" : "",
						onmouseover: function () {
							h(this, !0)
						},
						onmouseout: function () {
							h(this, !1)
						}
					});
				N.create("td", {
					className: this._css.stopDnDHandleClassHidden + " dojoDndHandle"
				}, c);
				var u = N.create("td", {
						className: this._css.stopIconColumnClass
					}, c);
				N.create("div", {
					className: this._css.stopIconClass + " dojoDndHandle",
					innerHTML: this._getLetter(a),
					"data-center-at": "true"
				}, u);
				var l = N.create("td", {
						className: this._css.esriStopGeocoderColumnClass
					}, c),
				d = N.create("div", {}, l),
				p = N.create("td", {
						className: this._css.stopIconRemoveColumnClass
					}, c);
				N.create("div", {
					className: this._css.stopIconRemoveClassHidden,
					role: "button",
					"data-remove": "true"
				}, p),
				this._dnd.insertNodes(!1, [c], o, n);
				var m = s.mixin({}, this.get("searchOptions"), {
						value: t.name,
						activeSourceIndex: this._globalGeocoder.activeSourceIndex
					}),
				_ = new L(m, d),
				g = s.hitch(this, function (t, e) {
						this._enqueue(function () {
							if (e !== t && _._stopReference && _._stopReference.name !== e) {
								var s = r.indexOf(this.stops, _._stopReference);
								this.stops[s] = {
									name: e
								},
								this._handle._remove(),
								this._removeSomeWaypoints(this._markWPsForRemovalAfterUserChangedStopSequence(s)),
								this._setStops(),
								this._clearDisplayBeforeSolve(),
								this._clearDisplayAfterSolve(),
								this.createRouteSymbols()
							}
						})
					});
				_._tr = c,
				_._stopReference = t,
				_.startup(),
				this.geocoders.splice(e, 0, _),
				this._geocoderEvents[c.id] = {
					blur: _.on("blur", function () {
						"" !== this.value && this._stopReference && !this._stopReference.feature && this.search()
					}),
					select: _.on("select-result", s.hitch(this, function (t) {
							var e = !0;
							if (t && (t.results || t.result)) {
								var s = this._dnd.getAllNodes(),
								i = r.indexOf(s, c),
								o = _.value,
								n = t.results && t.results.results && t.results.results.length ? t.results.results[0] : t.result;
								n ? (n.name = o, this.stops[i] = this._toPointGeometry(n), this.geocoders[i]._stopReference = this.stops[i], g("", o)) : (this.removeStop(i), this.set("directions", null), this._showMessage(C.widgets.directions.error.unknownStop.replace("<name>", t.target.get("value"))), this._clearRouteGraphics(), e = !1),
								e && this.getDirections()
							}
						})),
					suggest: _.on("suggest-results", function () {
						if (document.activeElement === this.inputNode)
							for (var t = b("LI[role='menuitem']", this.suggestionsNode), e = 0; e < t.length; e++)
								w.set(t[e], "tabindex", -1);
						else
							this._hideSuggestionsMenu()
					}),
					value: _.watch("value", function (t, e, s) {
						g(e, s)
					})
				},
				this._checkMaxStops(),
				this.setListIcons(),
				this._stopsRemovable(),
				this._optionsMenu(),
				this._sortGeocoders()
			},
			_blurGeocoders: function () {
				if (document.activeElement)
					for (var t = 0; t < this.geocoders.length; t++)
						if (this.geocoders[t].inputNode === document.activeElement) {
							this.geocoders[t]._hideSuggestionsMenu(),
							this.geocoders[t].inputNode.blur(),
							this._getStopCount() + this._getWaypointCount() > 1 && this.getDirections(!0);
							break
						}
			},
			_decorateEmptyAGOLGeocoderResponse: function (t) {
				return t && ", , " === t.name && (t.feature && t.feature.attributes && t.feature.attributes.Match_addr ? t.name = t.feature.attributes.Match_addr + ("POI" === t.feature.attributes.Addr_type && t.feature.attributes.City && -1 === t.feature.attributes.Match_addr.indexOf(t.feature.attributes.City) ? ", " + t.feature.attributes.City : "") : t.name = ""),
				t
			},
			_toPointGeometry: function (t) {
				var e = t.feature.geometry;
				if (e)
					if (e.getCentroid)
						t.feature.geometry = e.getCentroid();
					else if (e.getExtent) {
						var s = e.getExtent();
						s && (t.feature.geometry = s.getCenter())
					}
				return t
			},
			_removeLocateButtonVisibilityEvents: function () {
				for (var t = 0; t < this.geocoders.length; t++) {
					var e = this.geocoders[t];
					e._onMouseEnter && e._onMouseEnter.remove(),
					e._onMouseOut && e._onMouseOut.remove(),
					e._onKeyPress && e._onKeyPress.remove(),
					e._locateButton && (e._locateButton._onMouseEnter && e._locateButton._onMouseEnter.remove(), e._locateButton._onMouseOut && e._locateButton._onMouseOut.remove())
				}
			},
			_setLocateButtonVisibilityEvents: function () {
				this._removeLocateButtonVisibilityEvents();
				for (var t = this, e = function (e) {
					e instanceof FocusEvent ? this._geocoder._lbShown_f = !0 : this._geocoder._lbShown_g = !0,
					t._createLocateButton(this._geocoder, !0)
				}, i = function (e) {
					e instanceof FocusEvent ? this._geocoder._lbShown_f = !1 : this._geocoder._lbShown_g = !1,
					clearTimeout(this._destroyTimeout),
					this._destroyTimeout = setTimeout(s.hitch(this, function () {
								this._geocoder._lbShown_lb || this._geocoder._lbShown_f || t._destroyLocateButton(this._locateButton)
							}), 400)
				}, r = function () {
					this._geocoder._lbShown_g = !0,
					clearTimeout(this._destroyTimeout),
					this._destroyTimeout = setTimeout(s.hitch(this, function () {
								"" === this.value ? t._createLocateButton(this._geocoder, !0) : t._destroyLocateButton(this._locateButton)
							}), 400)
				}, o = function () {
					this._geocoder._lbShown_lb = !0,
					clearTimeout(this._geocoder._destroyTimeout)
				}, n = function () {
					this._geocoder._lbShown_lb = !1,
					clearTimeout(this._geocoder._destroyTimeout),
					this._geocoder._destroyTimeout = setTimeout(s.hitch(this, function () {
								this._geocoder._lbShown_g || this._geocoder._lbShown_f || t._destroyLocateButton(this._geocoder.inputNode._locateButton)
							}), 400)
				}, a = 0; a < this.geocoders.length; a++) {
					var h = this.geocoders[a];
					if (h && h.inputNode && (h.inputNode._geocoder = h, h._onMouseEnter = g(h.inputNode, f.enter, e), h._onMouseOut = g(h.inputNode, [f.leave, "blur"], i), h._onKeyPress = g(h.inputNode, "keydown", r), h.inputNode._locateButton)) {
						var c = h.inputNode._locateButton;
						c._onMouseEnter = g(c.domNode, f.enter, o),
						c._onMouseOut = g(c.domNode, f.leave, n)
					}
				}
			},
			_createLocateButton: function (e, i, o) {
				var a = new P,
				h = e;
				return h.inputNode._locateButton && h.inputNode._locateButton._locating ? a.resolve() : t(["esri/dijit/LocateButton"], s.hitch(this, function (t) {
						if (this._destroyLocateButton(h.inputNode._locateButton), h && !this._solveInProgress) {
							var e = N.create("div", {}, h.domNode);
							T.add(h.domNode, this._css.stopsInnerGeocoderClass);
							var c = new t({
									map: this.map,
									highlightLocation: !1,
									centerAt: !1,
									setScale: !1,
									useTracking: !1
								}, e);
							c.startup(),
							h.inputNode._locateButton = c,
							c.domNode._geocoder = h,
							this._setLocateButtonVisibilityEvents();
							var u = s.hitch(this, function () {
									c._locating = !0,
									h.set("value", ""),
									h.inputNode.placeholder = C.widgets.directions.retrievingMyLocation.toUpperCase()
								});
							c._onBeforeLocate = g(c._locateNode, n, u),
							c._onLocate = g(c, "locate", s.hitch(this, function (t) {
										c._locating = !1,
										t.graphic ? (i && this._destroyLocateButton(h.inputNode._locateButton), this.updateStop(new E(t.graphic.geometry), r.indexOf(this.geocoders, h)).then(s.hitch(this, function () {
													this.stopGraphics.length > 1 ? this.getDirections().always(function () {
														a.resolve(t)
													}) : a.resolve(t)
												}))) : (h.set("value", ""), h.inputNode.placeholder = C.widgets.directions.myLocationError.toUpperCase(), console.error(t.error), a.reject(t.error))
									})),
							o ? (u(), c.locate().then(null, a.reject)) : a.resolve()
						} else
							a.resolve()
					})),
				a.promise
			},
			_destroyLocateButton: function (t) {
				if (t) {
					var e = t.domNode._geocoder;
					clearTimeout(e._destroyTimeout),
					t._locating ? e._destroyTimeout = setTimeout(s.hitch(this, function () {
								e._lbShown_lb || e._lbShown_f || this._destroyLocateButton(t)
							}), 100) : (t.clear(), t._onBeforeLocate.remove(), t._onLocate.remove(), t._onMouseEnter && t._onMouseEnter.remove(), t._onMouseOut && t._onMouseOut.remove(), t.destroy(), e.inputNode && (e.inputNode._locateButton = null, e._setPlaceholder(e.activeSourceIndex)))
				}
			},
			_sortStops: function () {
				this.stops.length && (this.stops.sort(s.hitch(this, function (t, e) {
							for (var s, i, r = 0; r < this.get("geocoders").length; r++)
								this.geocoders[r]._stopReference === t ? s = r : this.geocoders[r]._stopReference === e && (i = r);
							return s > i ? 1 : i > s ? -1 : 0
						})), this._setStops())
			},
			_getCandidate: function (t) {
				var e = new P,
				i = typeof t;
				if (t)
					if ("object" === i && t.hasOwnProperty("feature") && t.hasOwnProperty("name"))
						t.feature.attributes && void 0 !== t.feature.attributes.displayName && !this._isStopAWaypoint(t) && (t.name = t.feature.attributes.displayName, t.feature.attributes.Name = this._userDefinedStopName), t.name = this._isStopAWaypoint(t) ? this._waypointName : String(t.name), "point" !== t.feature.geometry.type && (t.feature.geometry = new Z([t.feature.geometry.x, t.feature.geometry.y], this.map.spatialReference)), e.resolve(t);
					else if ("object" === i && t.hasOwnProperty("address") && t.hasOwnProperty("location")) {
						var r = this._globalGeocoder._hydrateResult(t);
						e.resolve(r)
					} else if ("object" !== i || !t.hasOwnProperty("name") || null !== t.name && "" !== t.name)
						if (t instanceof E && t.attributes && (void 0 !== t.attributes.Name || t.attributes.isWaypoint)) {
							var o = this._addStopWrapperToGraphic(t, t.attributes.isWaypoint ? this._waypointName : String(t.attributes.Name));
							String(t.attributes.Name) && (o.feature.attributes.Name = this._userDefinedStopName),
							e.resolve(o)
						} else
							"object" === i && t.hasOwnProperty("name") && (t = String(t.name)), this._reverseGeocode(t).then(e.resolve, e.reject);
					else
						e.resolve(s.clone(this._emptyStop));
				else
					e.resolve(s.clone(this._emptyStop));
				return e.promise
			},
			_reverseGeocode: function (t) {
				var e,
				i = new P,
				r = t.geometry ? t.geometry : t;
				if (this._globalGeocoder) {
					var o = s.hitch(this, function (t) {
							var e = new P,
							s = 500;
							if (this.map) {
								var i = this.map.toScreen(r);
								if (i.x += q._calculateClickTolerance([t]), this.map.spatialReference.isWebMercator())
									s = Math.abs(this.map.toMap(i).x - r.x), e.resolve(s);
								else if (4326 === this.map.spatialReference.wkid)
									s = Math.abs(F.geographicToWebMercator(this.map.toMap(i)).x - F.geographicToWebMercator(r).x), e.resolve(s);
								else if (this._geometryService) {
									var o = new nt;
									o.distanceUnit = ot.UNIT_METER,
									o.geometry1 = r,
									o.geometry2 = this.map.toMap(i),
									this._geometryService.distance(o, function (t) {
										s = t,
										e.resolve(s)
									}, function () {
										e.resolve(s)
									})
								}
							}
							return e.promise
						}),
					n = [],
					a = this._globalGeocoder.sources,
					h = function () {
						a[e].featureLayer && n.push(o(a[e].featureLayer).then(s.hitch(a[e], function (t) {
									this.searchQueryParams = s.mixin(this.searchQueryParams, {
											distance: t
										})
								})))
					};
					if ("all" === this._globalGeocoder.activeSourceIndex)
						for (e = 0; e < a.length; e++)
							h();
					else
						e = this._globalGeocoder.activeSourceIndex, h();
					A(n).always(s.hitch(this, function () {
							this._globalGeocoder.search(r).then(s.hitch(this, function (s) {
									var o = !1;
									if (s) {
										var n,
										a = null;
										for (e = 0; e < this._globalGeocoder.sources.length; e++)
											if (s[e] && s[e].length) {
												n = s[e];
												break
											}
										if (n.length && (o = !0, a = n[0], this._globalGeocoder.sources[e].featureLayer)) {
											var h = Number.POSITIVE_INFINITY;
											for (e = 0; e < n.length; e++) {
												n[e] = this._toPointGeometry(n[e]);
												var c = H.geodesicLengths([new z({
																paths: [[F.xyToLngLat(r.x, r.y), F.xyToLngLat(n[e].feature.geometry.x, n[e].feature.geometry.y)]],
																spatialReference: {
																	wkid: this.map.spatialReference.wkid
																}
															})], "esriMeters")[0];
												h > c && (h = c, a = n[e])
											}
										}
										a = this._decorateEmptyAGOLGeocoderResponse(a),
										a && "" !== a.name && null !== a.name && void 0 !== a.name ? (a.name = String(a.name), isNaN(r.x) || isNaN(r.y) || this.map.spatialReference.wkid !== r.spatialReference.wkid ? !a.feature.geometry || isNaN(a.feature.geometry.x) || isNaN(a.feature.geometry.y) ? (this._showMessage(C.widgets.directions.error.locator), i.reject(new Error(C.widgets.directions.error.locator))) : i.resolve(a) : (a.feature.geometry = r, i.resolve(a))) : o = !1
									}
									o || (t instanceof Z ? t = new E(t) : t instanceof Array && (t = new E(new Z(t[0], t[1]))), t instanceof E ? this._decorateUngeocodedStop(t).then(i.resolve, i.reject) : (this._showMessage(C.widgets.directions.error.unknownStop.replace("<name>", t.toString())), i.reject(new Error(C.widgets.directions.error.unknownStop.replace("<name>", t.toString())))))
								}))
						}))
				} else
					this._showMessage(C.widgets.directions.error.locatorUndefined), i.reject(new Error(C.widgets.directions.error.locatorUndefined));
				return i.promise
			},
			_updateStop: function (t, e, i) {
				var r = new P;
				return this.stops && this.stops[e] ? t instanceof E && t.attributes && t.attributes.isWaypoint && (!e || e === this.stops.length - 1 || this._getStopCount() < 2) ? (this._showMessage(C.widgets.directions.error.waypointShouldBeInBetweenStops), r.reject(new Error(C.widgets.directions.error.waypointShouldBeInBetweenStops))) : this._getCandidate(t).then(s.hitch(this, function (t) {
						var s = t.feature,
						o = s ? s.geometry : null,
						n = this.stops[e].feature,
						a = n ? n.geometry : null,
						h = o && a && (o.x !== a.x || o.y !== a.y) || !o && a;
						this.stops[e] = t,
						this.geocoders[e] || this._createGeocoder(t, e);
						var c = this.geocoders[e];
						c._stopReference = t,
						c._tr.style.display = this._isStopAWaypoint(t) ? "none" : "",
						c.value = t.name,
						c.inputNode && (c.inputNode.value = t.name),
						h && this.autoSolve && this._getStopCount() + this._getWaypointCount() > 1 || i ? this._getDirections().then(r.resolve, r.reject) : (this._setStops(), r.resolve(t))
					}), s.hitch(this, function (t) {
						r.reject(t)
					})) : (this._showMessage(C.widgets.directions.error.couldNotUpdateStop), r.reject(new Error(C.widgets.directions.error.couldNotUpdateStop))),
				this._optionsMenu(),
				r.promise
			},
			_renderDirections: function () {
				var t = this.get("directions"),
				e = "";
				if (this._resultsNode) {
					e += '<div class="' + this._css.clearClass + '"></div>',
					e += this._renderDirectionsSummary(t),
					e += '<div class="' + this._css.clearClass + '"></div>',
					e += '<div class="' + this._css.routesClass + '">',
					e += this._renderDirectionsTable(t),
					e += "<div class='esriPrintFooter'>" + C.widgets.directions.printDisclaimer + "</div>",
					e += "</div>",
					this._resultsNode && (this._resultsNode.innerHTML = e),
					this._disconnectResults();
					var i = b("[data-segment]", this._resultsNode);
					i && i.length && r.forEach(i, s.hitch(this, function (t) {
							this._resultEvents.push(g(t, f.enter, s.hitch(this, function () {
										if (!this._focusedDirectionsItem) {
											var e = parseInt(w.get(t, "data-segment"), 10);
											this.highlightSegment(e)
										}
									}))),
							this._resultEvents.push(g(t, "focusout", s.hitch(this, function () {
										this._focusedDirectionsItem = null,
										this.unhighlightSegment(!0)
									}))),
							this._resultEvents.push(g(t, f.leave, this.unhighlightSegment)),
							this._resultEvents.push(g(t, "click, keydown", s.hitch(this, function (e) {
										e && ("click" === e.type || "keydown" === e.type && e.keyCode === m.ENTER) && (this._focusedDirectionsItem !== t ? this.selectSegment(parseInt(w.get(t, "data-segment"), 10)) : (t.blur(), this.map.infoWindow.hide(), this._focusedDirectionsItem = null, this.unhighlightSegment(!0)))
									})))
						}))
				}
			},
			_renderDirectionsSummary: function (t) {
				var e = "",
				i = s.hitch(this, function () {
						for (var e = {}, s = this._getDirectionsTimeAttribute(), i = this._getTimeNeutralAttribute() || {}, r = 0, o = 0, n = this.get("stops"), a = n.length - 1; a >= 0; a--)
							if (null !== n[a].feature.attributes.ArriveCurbApproach) {
								e = n[a].feature.attributes;
								break
							}
						for (var h in e)
							e.hasOwnProperty(h) && (h === "Cumul_" + s.name && (r = e[h]), h === "Cumul_" + i.name && (o = e[h]));
						var c = "esriTrafficLabelHidden",
						u = C.widgets.directions.noTraffic,
						l = t.totalTime - t.totalDriveTime,
						d = (o - this._convertCostValue(l, s.units, i.units)) / (r - l || 1);
						d > 0 && .8 > d ? (c = "esriTrafficLabelHeavy", u = C.widgets.directions.heavyTraffic) : 1 === d ? (c = "esriTrafficLabelNone", u = C.widgets.directions.noTraffic) : d > 1.25 && (c = "esriTrafficLabelLight", u = C.widgets.directions.ligthTraffic);
						var p = this._formatTime(o, !1, i.units);
						return {
							label: u,
							labelClass: c,
							ratio: d,
							noTrafficCostStr: 1 !== d && p ? p + " " + C.widgets.directions.onAverage + "<br>" : "",
							trafficCost: r,
							noTrafficCost: o,
							timeAtt: s
						}
					});
				if (t.totalLength || t.totalTime) {
					var r = i(),
					o = this._getImpedanceAttribute();
					if (e += "<div class='" + this._css.resultsSummaryClass + "' data-full-route='true'><div class='esriImpedanceCost'>", this._isTimeUnits(o.units))
						e += this._formatTime(r.trafficCost, !0, r.timeAtt.units) + "<div class='esriImpedanceCostHrMin'><div class='esriImpedanceCostHr'>" + C.widgets.directions.time.hr + "</div><div class='esriImpedanceCostMin'>" + C.widgets.directions.time.min + "</div></div></div><div class='esriOtherCosts'>" + (r.noTrafficCost ? "<div class='esriTrafficLabel " + r.labelClass + "'>" + r.label + "</div>" + r.noTrafficCostStr : "") + this._formatDistance(t.totalLength);
					else {
						var n = C.widgets.directions.units[this.directionsLengthUnits],
						a = n ? n.name : "",
						h = this.serviceDescription && this.serviceDescription.currentVersion >= 10.6,
						c = this.directions.features[0].attributes;
						e += M.format(t.totalLength, {
							places: 1
						}) + "<div class='esriImpedanceCostDist'>" + a + "</div></div><div class='esriOtherCosts'>" + (r.noTrafficCost && h ? "<div class='esriTrafficLabel " + r.labelClass + "'>" + r.label + "</div>" + r.noTrafficCostStr : "") + this._formatTime(t.totalTime) + " " + ("none" !== this.startTime && r.noTrafficCost && h ? C.widgets.directions.atTheMoment + " " + this._toSpatiallyLocalTimeString(c.arriveTimeUTC, c.ETA) : "")
					}
					e += "</div></div>"
				}
				return e
			},
			_renderDirectionsTable: function (t) {
				for (var e = 0, s = 0, i = 0, r = '<table summary="' + t.routeName + '"><tbody role="menu">', o = 0; o < t.features.length; o++) {
					var n = t.features[o].attributes;
					"esriDMTDepart" === n.maneuverType && (i = 0, s = 0),
					i += n.length,
					s += n.time,
					e += n.time,
					r += this._renderDirectionsItemTR(t.features[o], i, s, e)
				}
				return r += "</tbody></table>"
			},
			_renderDirectionsItemTR: function (t, e, s, i) {
				var o,
				n,
				a = this.directions,
				h = a ? r.indexOf(a.features, t) : -1,
				c = "",
				u = this._css.routeClass,
				l = t._associatedStopWithReturnToStart && t._associatedStopWithReturnToStart.attributes,
				d = t.attributes;
				if (h > -1) {
					d && (d.step = h + 1),
					d.maneuverType && (u += " " + d.maneuverType),
					l && null === l.ArriveCurbApproach && null !== l.DepartCurbApproach ? u += " " + this._css.routeOriginClass : l && null !== l.ArriveCurbApproach && null === l.DepartCurbApproach && (u += " " + this._css.routeDestinationClass + " " + this._css.routeLastClass),
					c += '<tr tabindex="0" role="menuitem" class="' + u + " " + this._css.routeZoomClass + '" data-segment="' + h + '">',
					c += '<td class="' + this._css.routeIconColumnClass + '">',
					c += '<div class="' + this._css.routeIconClass + '">',
					c += this._getLetter(t._associatedStop),
					c += "</div>",
					c += "</td>",
					c += '<td class="' + this._css.routeTextColumnClass + '">',
					c += '<div class="' + this._css.routeInfoClass + '">',
					c += '<div class="' + this._css.routeTextClass + '">';
					var p,
					m = (a.strings[h] || []).slice();
					if ("esriDMTDepart" === d.maneuverType || "esriDMTStop" === d.maneuverType)
						for (p = 0; p < this.stops.length; p++)
							this.stops[p] && this.stops[p].name && m.push({
								string: this.stops[p].name
							});
					if (m) {
						var _ = d.text;
						for (p = 0; p < m.length; p++)
							_ = this._boldText(_, m[p].string);
						d.formattedText = _
					} else
						d.formattedText = d.text;
					if ("esriDMTStop" === d.maneuverType && (e || s) || "esriDMTDepart" === d.maneuverType && 0 === h) {
						o = this._formatDistance(e - d.length, !0),
						n = this._formatTime(s - d.time);
						var g = this._formatTime(d.time),
						f = this._formatDistance(d.length, !0);
						d.formattedText += "<div class='esriRouteTextColumnCumulative'>" + o + (o && n ? " &middot; " : "") + n + (o || n ? "<br>" : "") + (g ? C.widgets.directions.serviceTime + ":&nbsp;" + g : "") + (g && f ? "<br>" : "") + (f ? C.widgets.directions.serviceDistance + ":&nbsp;" + f : "") + "</div>"
					}
					c += "<strong>" + M.format(d.step) + ".</strong> " + d.formattedText,
					c += "</div>",
					o = this._formatDistance(d.length, !0),
					n = this._formatTime(d.time),
					"esriDMTStop" !== d.maneuverType && "esriDMTDepart" !== d.maneuverType || !this.routeParams.startTime || -22091616e5 === d.ETA ? o && (c += '<div class="' + this._css.routeLengthClass + '">', c += o, n && (c += "&nbsp;&middot;<wbr>&nbsp;" + n), c += "</div>") : c += '<div class="' + this._css.routeLengthClass + '">' + this._toSpatiallyLocalTimeString(d.arriveTimeUTC, d.ETA) + "</div>",
					c += "</div>",
					c += "</td>",
					c += "</tr>"
				}
				return c
			},
			_toSpatiallyLocalTimeString: function (t, e) {
				var s = new Date(e),
				r = new Date(s.getTime() + 60 * s.getTimezoneOffset() * 1e3),
				o = i.date.locale.format(r, {
						selector: "time"
					}),
				n = "";
				if (t) {
					var a = (e - t) / 1e3 / 60 / 60,
					h = Math.floor(a),
					c = 60 * (a - h),
					u = C.widgets.directions.GMT + (0 > h ? "" : "+") + M.format(h, {
							pattern: "00"
						}) + M.format(c, {
							pattern: "00"
						});
					n = o + " " + u
				} else
					n = "now" === this.startTime ? i.date.locale.format(s, {
							selector: "time"
						}) : o;
				return n
			},
			_addStopWrapperToGraphic: function (t, e) {
				return {
					extent: new V({
						xmin: t.geometry.x - .25,
						ymin: t.geometry.y - .25,
						xmax: t.geometry.x + .25,
						ymax: t.geometry.y + .25,
						spatialReference: t.geometry.spatialReference
					}),
					feature: t,
					name: e
				}
			},
			_clearBarriersGraphics: function () {
				this._barriersLayer.clear(),
				this._polylineBarriersLayer.clear(),
				this._polygonBarriersLayer.clear()
			},
			_showBarriers: function () {
				this._clearBarriersGraphics();
				var t = this.routeParams,
				e = t.barriers && t.barriers.features || [],
				i = t.polylineBarriers && t.polylineBarriers.features || [],
				o = t.polygonBarriers && t.polygonBarriers.features || [];
				r.forEach(e, s.hitch(this, function (t) {
						this._barriersLayer.add(t)
					})),
				r.forEach(i, s.hitch(this, function (t) {
						this._polylineBarriersLayer.add(t)
					})),
				r.forEach(o, s.hitch(this, function (t) {
						this._polygonBarriersLayer.add(t)
					})),
				this._barriersLayer.refresh(),
				this._polylineBarriersLayer.refresh(),
				this._polygonBarriersLayer.refresh()
			},
			_showRoute: function (t) {
				this._clearDisplayAfterSolve();
				var e = t.routeResults[0].directions,
				s = new P;
				if (e) {
					this.set("solveResult", t),
					this.set("directions", e);
					var i,
					r,
					o = t.routeResults[0].stops;
					if (o && o.length) {
						var n = [];
						for (i = 0; i < o.length; i++) {
							var a = o[i];
							a.attributes.isWaypoint = a.attributes.Name === this._waypointName || a.attributes.isWaypoint;
							var h = this._addStopWrapperToGraphic(a, a.attributes.Name);
							this.stops[i] && this.stops[i].feature && this.stops[i].feature.attributes && this.stops[i].feature.attributes.Name === this._userDefinedStopName && (h.feature.attributes.Name = this._userDefinedStopName),
							this._returnToStartStop && this._returnToStartStop._resultsStopIndex === i ? this._returnToStartStop = h : n.push(h)
						}
						if (this.stops.length > n.length)
							for (i = 0; i < this.stops.length; i++)
								this.stops[i].feature || "" !== this.stops[i].name || n.splice(i, 0, this._emptyStop);
						for (this.stops = n, i = 0; i < this.stops.length; i++)
							this._updateStop(this.stops[i], i);
						this._setStops(),
						this._setMenuNodeValues()
					}
					this.set("mergedRouteGraphic", new E(e.mergedGeometry, this.get("routeSymbol")));
					var c = [],
					u = [],
					l = 0;
					for (i = 0; i < e.featuresWithWaypoints.length; i++) {
						var d = e.featuresWithWaypoints[i];
						if ("esriDMTDepart" === d.attributes.maneuverType)
							for (r = 0; r < this.stops.length; r++)
								if (this.stops[r].feature === d._associatedStop) {
									l = r + 1;
									break
								}
						d.setSymbol(this.get("routeSymbol")),
						this._routeLayer.add(d),
						c.push(d);
						var p = d.getDojoShape();
						p && p.moveToBack();
						var m = new E(d.geometry, this._symbolEventPaddingDirections, d.attributes);
						if (m._nextStopIndex = l - .5, m._isSnapFeature = !0, d._associatedSnapFeature = m, this._waypointsEventLayer.add(m), "esriDMTDepart" !== d.attributes.maneuverType) {
							var _ = d.geometry.getPoint(0, 0);
							if (_) {
								var g = new E(_, this.maneuverSymbol);
								g._directionsFeature = d._associatedFeatureNoWaypoints,
								u.push(g),
								this._waypointsEventLayer.add(u[u.length - 1])
							}
						}
					}
					for (this.set("displayedRouteGraphics", c), this.set("displayedManeuverPointGraphics", u), this._renderDirections(), i = 0; i < this.stops.length; i++)
						if (this._isStopAWaypoint(this.stops[i]) && this._modifiedWaypointIndex === i) {
							for (r = 0; r < e.featuresWithWaypoints.length; r++) {
								var f = e.featuresWithWaypoints[r];
								if (f._associatedStop === this.stops[i].feature) {
									("esriDMTStop" === f.attributes.maneuverType || "esriDMTDepart" === f.attributes.maneuverType) && (this.stops[i].feature.geometry.x = f.geometry.paths[0][0][0], this.stops[i].feature.geometry.y = f.geometry.paths[0][0][1]);
									break
								}
							}
							this._modifiedWaypointIndex = null;
							break
						}
					S.set(this._savePrintBtnContainer, "display", "inline-block"),
					this.onDirectionsFinish(t),
					s.resolve()
				} else {
					var v = t.routeResults[0].route;
					v.setSymbol(this.routeSymbol),
					this._routeLayer.add(v),
					this._incrementalRouteSegment = v,
					s.resolve()
				}
				return this._moveLayersToFront(),
				this.createRouteSymbols(),
				s.promise
			},
			_setGeocodersStopReference: function () {
				if (this.geocoders)
					for (var t = 0; t < this.geocoders.length; t++)
						this.geocoders[t] && this.stops[t] && (this.geocoders[t]._stopReference = this.stops[t])
			},
			_setStops: function () {
				this._setGeocodersStopReference(),
				this.createRouteSymbols(),
				this._set("stops", this.stops),
				this.onStopsUpdate(this.stops)
			},
			_getCandidates: function (t) {
				var e = [];
				if (t && t.length) {
					for (var s = 0; s < t.length; s++)
						e.push(this._getCandidate(t[s]));
					return A(e)
				}
				var i = new P;
				return i.resolve([]),
				i.promise
			},
			_clearResultsHTML: function () {
				this._resultsNode.innerHTML = "",
				S.set(this._savePrintBtnContainer, "display", "none")
			},
			_showSegmentPopup: function (t) {
				if (t && this.get("showSegmentPopup") && this.get("map").infoWindow) {
					var e = t.geometry,
					s = e.getPoint(0, 0),
					i = new E(s, null, t.attributes, this.get("segmentInfoTemplate")),
					r = this.get("map").infoWindow;
					r.setFeatures([i]),
					r.show(s)
				}
			},
			_addStopButton: function () {
				this.addStop().then(s.hitch(this, function () {
						this.get("focusOnNewStop") && this.geocoders[this.stops.length - 1].focus()
					}))
			},
			_sortGeocoders: function () {
				var t = this._dnd.getAllNodes();
				this.geocoders.sort(s.hitch(this, function (e, s) {
						return e.domNode && e.domNode.parentNode && e.domNode.parentNode.parentNode && s.domNode && s.domNode.parentNode && s.domNode.parentNode.parentNode ? r.indexOf(t, e.domNode.parentNode.parentNode) > r.indexOf(t, s.domNode.parentNode.parentNode) ? 1 : -1 : 0
					})),
				this.stops.length === this.geocoders.length && this._sortStops();
				for (var e = 0; e < this.geocoders.length; e++)
					this.geocoders[e] && this.geocoders[e].inputNode && (this.geocoders[e].inputNode.title = C.widgets.directions.stopNoTitle + (e + 1));
				this._setLocateButtonVisibilityEvents()
			},
			_disconnectResults: function () {
				if (this._resultEvents && this._resultEvents.length)
					for (var t = 0; t < this._resultEvents.length; t++)
						this._resultEvents[t] && this._resultEvents[t].remove();
				this._resultEvents = []
			},
			_formatArbitraryCostsForRouteTooltip: function (t) {
				var e = "";
				for (var s in t)
					if (0 === s.indexOf("Total_") && t.hasOwnProperty(s)) {
						var i = this._getCostAttribute(s.substr(6));
						i && (e += this._isTimeUnits(i.units) ? this._formatTime(t[s], !1, i.units) : this._formatDistance(t[s], !0, i.units), e += e ? " &middot; " : "")
					}
				return e && (e = C.widgets.directions.toNearbyStops + ": <b>" + e.substr(0, e.length - 10) + "</b>"),
				e
			},
			_formatTime: function (t, e, s) {
				s || (s = (this._getDirectionsTimeAttribute() || {}).units);
				var i,
				r,
				o = "",
				n = Math.round(this._convertCostValue(t, s, "esriNAUMinutes"));
				return i = Math.floor(n / 60),
				r = Math.floor(n % 60),
				e ? o = M.format(i, {
						pattern: 100 > i ? "00" : "000"
					}) + ":" + M.format(r, {
						pattern: "00"
					}) : (i && (o += i + " " + C.widgets.directions.time.hr + " "), o += i || r ? r + " " + C.widgets.directions.time.min : ""),
				o
			},
			_formatDistance: function (t, e, s) {
				s || (s = this.directionsLengthUnits);
				var i = this.directionsLengthUnits,
				r = C.widgets.directions.units[i],
				o = i.replace("esri", "").toLowerCase(),
				n = this._convertCostValue(t, s, i);
				return r && (o = e ? r.abbr : r.name),
				n ? M.format(n, {
					locale: "root",
					places: 2
				}) + " " + o : ""
			},
			_createToolbars: function () {
				this.editToolbar || (this.editToolbar = new ut(this.map)),
				this.drawToolbar || (this.drawToolbar = new lt(this.map), C.toolbars.draw.freehand = C.widgets.directions.lineBarrierFreehand, this.drawToolbar.onDrawComplete = s.hitch(this, function (t) {
							var e = new E(t.geometry, null, {
									BarrierType: 0
								});
							H.geodesicLengths([t.geographicGeometry], O.METERS)[0] > 1 && (this._polylineBarriersLayer.add(e), this.routeParams.polylineBarriers || (this.routeParams.polylineBarriers = new st), this.routeParams.polylineBarriers.features.push(e), this._clearStopsStatusAttr(), this.getDirections(!0))
						}))
			},
			_destroyGlobalGeocoder: function () {
				this._globalGeocoder && (this._globalGeocoder.destroy(), this._globalGeocoder = null)
			},
			_createGlobalGeocoder: function () {
				var t = new P;
				return this._globalGeocoder = new L(this.get("searchOptions")),
				g.once(this._globalGeocoder, "load", t.resolve, t.reject),
				this._globalGeocoder.startup(),
				t.promise
			},
			_init: function () {
				var t = new P;
				return this.set("loaded", !1),
				this._enableButton(this._getDirectionsButtonNode, !1),
				S.set(this._saveAsButton, "display", "none"),
				this.clearMessages(),
				this.get("map").loaded ? this._configure().always(t.resolve) : g.once(this.get("map"), "load", s.hitch(this, function () {
						this._configure().always(t.resolve)
					})),
				t.promise
			},
			_setDefaultStops: function () {
				var t = new P;
				return this.defaults.stops && this.defaults.stops.length ? this._updateStops(this.defaults.stops).then(s.hitch(this, function () {
						this._removeEmptyStops(),
						t.resolve()
					}), t.reject) : t.resolve(),
				t.promise
			},
			_configure: function () {
				var t = new P;
				return this._handle && this._handle._remove(),
				this._createDnD(),
				this._createDepartAtControls(),
				this._setSearchOptions(),
				this._createGlobalGeocoder().then(s.hitch(this, function () {
						this._createToolbars(),
						this._usingAGOL() || (this.geometryTaskUrl = null, this.printTaskUrl = null),
						this._createGeometryTask(),
						this._createPrintTask(),
						this._showActivateButton(),
						this._showBarriersButton(),
						this._createTravelModesDDL(),
						this._createSearchSourceDDL();
						var e = [this._createRouteTask(), this._setDefaultStops()];
						A(e).then(s.hitch(this, function () {
								this._setDefaultUnits(),
								this._setTrafficOptions(),
								this._setMenuNodeValues(),
								this._setupEvents();
								var e = this.directionsLanguage || this.userOptions.routeParams && this.userOptions.routeParams.directionsLanguage || i.locale.toLowerCase();
								this._setDirectionsLanguageByLocale(e),
								this._setupTravelModes().then(s.hitch(this, function () {
										this.set("loaded", !0),
										this.onLoad(),
										t.resolve(!0)
									}), function (e) {
									t.reject(e)
								})
							}), function (e) {
							t.reject(e)
						})
					}), function (e) {
					t.reject(e)
				}),
				this._naRouteSharing = null,
				t.promise
			},
			_setDirectionsLanguageByLocale: function (t) {
				var e = this.serviceDescription.directionsSupportedLanguages,
				s = function (t) {
					if (e)
						for (var s = 0; s < e.length; s++)
							if (e[s].toLowerCase().substr(0, 2) === t)
								return e[s];
					return null
				},
				i = s(t);
				return i || (t = t.substr(0, 2), i = s(t)),
				this.directionsLanguage = i,
				this.routeParams.directionsLanguage = i,
				i
			},
			_getStopSymbol: function (t, e) {
				var s = t && (t.feature && t.feature.attributes || t.attributes),
				i = this.stopSymbol;
				return s && (i = this._isStopAWaypoint(t) ? this.waypointSymbol : void 0 === s.Status || 0 === s.Status || 6 === s.Status ? null === s.ArriveCurbApproach && null !== s.DepartCurbApproach ? this.get(e ? "fromSymbolDrag" : "fromSymbol") : null !== s.ArriveCurbApproach && null === s.DepartCurbApproach ? this.get(e ? "toSymbolDrag" : "toSymbol") : this.get(e ? "stopSymbolDrag" : "stopSymbol") : this.get(e ? "unreachedSymbolDrag" : "unreachedSymbol")),
				i
			},
			_addTrafficLayer: function () {
				this.trafficLayer && !this._trafficLayerAdded && this.map && (this.map.addLayer(this.trafficLayer), this.trafficLayer.show(), this._trafficLayerAdded = !0)
			},
			_toggleUnits: function (t) {
				t.target === this._useMilesNode ? this.setDirectionsLengthUnits(O.MILES) : t.target === this._useKilometersNode && this.setDirectionsLengthUnits(O.KILOMETERS)
			},
			_toggleCheckbox: function (t) {
				var e = w.get(t.target, "checked");
				t.target === this._findOptimalOrderNode ? this.set("optimalRoute", e) : t.target === this._useTrafficNode ? this.set("traffic", e) : t.target === this._returnToStartNode && this.set("returnToStart", e)
			},
			_isStopLocated: function (t) {
				return t && t.feature && t.feature.attributes && (!t.feature.attributes.Status || 6 === t.feature.attributes.Status)
			},
			_configureRouteOptions: function () {
				var t,
				e = this.get("routeParams");
				if (this.get("directionsLengthUnits") ? e.directionsLengthUnits = this.get("directionsLengthUnits") : this.set("directionsLengthUnits", e.directionsLengthUnits), e.findBestSequence = this.get("optimalRoute"), e.findBestSequence)
					for (e.preserveFirstStop = this._isStopLocated(this.stops[0]), e.preserveLastStop = !this.returnToStart && this._isStopLocated(this.stops[this.stops.length - 1]) || this.returnToStart && this._isStopLocated(this.stops[0]), t = 0; t < this.stops.length; )
						this._isStopAWaypoint(this.stops[t]) ? this._removeStop(t, !0, !0) : t++;
				if (!this.returnToStart && this.stops.length && this._isStopAWaypoint(this.stops[this.stops.length - 1]))
					for (t = this.stops.length - 1; t > 0; )
						this._isStopAWaypoint(this.stops[t]) && this._removeStop(t, !0, !0), t--;
				if (this._isTimeUnits(this._getImpedanceAttribute().units) || this.serviceDescription && this.serviceDescription.currentVersion >= 10.6)
					if (e.useTimeWindows = !0, "now" === this.startTime)
						e.timeWindowsAreUTC = !0, e.startTimeIsUTC = !0, e.startTime = new Date;
					else if ("none" === this.startTime)
						e.startTime = null;
					else {
						e.timeWindowsAreUTC = !1,
						e.startTimeIsUTC = !1;
						var s = this._departAtTime,
						i = 60 * s.getValue().getTimezoneOffset() * 1e3,
						o = new Date(s.getValue().getTime() + this._departAtDate.getValue().getTime() - i),
						n = new Date(o.getTime() - 60 * o.getTimezoneOffset() * 1e3);
						e.startTime = n
					}
				else
					e.startTime = null, e.useTimeWindows = !1;
				var a = this._getImpedanceAttribute(),
				h = this._getTimeNeutralAttribute(),
				c = this._getDirectionsTimeAttribute(),
				u = this.routeParams.accumulateAttributes || this.serviceDescription.accumulateAttributeNames;
				h && -1 === r.indexOf(u, h.name) && u.push(h.name),
				!this._isTimeUnits(a.units) && this.serviceDescription && this.serviceDescription.currentVersion >= 10.6 && c && -1 === r.indexOf(u, c.name) && u.push(c.name),
				this.routeParams.accumulateAttributes = u,
				e.returnStops = !0;
				var l = [];
				for (t = 0; t < this.stopGraphics.length; t++)
					if (this.stopGraphics[t] && (l.push(new E(this.stopGraphics[t].toJson())), t)) {
						var d = l[0].attributes,
						p = l[t].attributes;
						for (var m in p)
							p.hasOwnProperty(m) && !d.hasOwnProperty(m) && (d[m] = null)
					}
				if (this.get("returnToStart") && this.stopGraphics.length && this._isStopLocated(this.stops[0])) {
					var _ = new E(this.stopGraphics[0].toJson());
					this._returnToStartStop = this._addStopWrapperToGraphic(_, _.attributes.Name),
					l.push(_)
				} else
					this._returnToStartStop = null;
				if (e.stops.features = l, h)
					for (t = 0; t < l.length; t++) {
						var g = l[t].attributes["Attr_" + a.name];
						l[t].attributes["Attr_" + h.name] = this._convertCostValue(g, a.units, h.units)
					}
				this.set("routeParams", e)
			},
			_configureRoute: function () {
				var t = new P;
				if (t.promise.always(s.hitch(this, function () {
							this._checkMaxStops()
						})), this.createRouteSymbols(), this._configureRouteOptions(), this.routeParams.returnRoutes && this._incrementalSolveStopRange) {
					var e = this._incrementalSolveStopRange;
					e.start < e.end ? this.routeParams.stops.features = this.routeParams.stops.features.slice(e.start, e.end + 1) : e.start > e.end && (this.routeParams.stops.features = this.routeParams.stops.features.slice(e.start, this.routeParams.stops.features.length - 1).concat(this.routeParams.stops.features.slice(0, e.end + 1)))
				} else
					this._incrementalSolveStopRange = null;
				var i,
				r,
				o,
				n = {},
				a = this.routeParams.stops.features;
				for (i = 0; i < a.length; i++)
					o = a[i].attributes.address, r = (i === this._handle._index && !this._handle.attributes.isWaypoint && this._incrementalSolveStopRange ? this._waypointName : o) + "_" + this._stopSequence++, a[i].attributes.Name = r, n[r] = o, delete a[i].attributes.address, delete a[i].attributes.isWaypoint, delete a[i].attributes.Status;
				this._solveInProgress = !0;
				var h = {
					_incrementalSolveStopRange: this._incrementalSolveStopRange
				};
				return this.routeTask.solve(this.routeParams, s.hitch(this, function (e) {
						this._solveResultProcessing(e, n, h).then(t.resolve, t.reject)
					}), s.hitch(this, function (e) {
						s.mixin(this, h),
						this._solveInProgress = !1;
						for (var i = 0; i < this.stops.length; i++)
							this.stops[i].feature && (this.stops[i].feature.attributes = s.mixin(this.stops[i].feature.attributes, {
										Status: 5
									}));
						this.set("directions", null),
						this._clearDisplayAfterSolve(),
						this.createRouteSymbols(),
						this._routeTaskError(e),
						t.reject(e)
					})),
				t.promise
			},
			_solveResultProcessing: function (t, e, i) {
				var r = new P;
				s.mixin(this, i),
				this._solveInProgress = !1;
				var o,
				n = t.routeResults[0],
				a = n.directions,
				h = n.stops;
				if (a) {
					this._solverMessages = t.messages;
					var c = function (t) {
						for (var s in e)
							if (t && 0 === a.routeName.indexOf(s) || !t && a.routeName.indexOf(s) > 0)
								return e[s];
						return ""
					},
					u = c(!0),
					l = c(!1);
					for (a.routeName = (u !== this._waypointName ? u : C.widgets.directions.waypoint) + " — " + (l !== this._waypointName ? l : C.widgets.directions.waypoint), o = 0; o < a.features.length; o++) {
						var d = a.features[o].attributes;
						if ("esriDMTDepart" === d.maneuverType || "esriDMTStop" === d.maneuverType)
							for (var p in e)
								if (e.hasOwnProperty(p) && d.text.indexOf(p) > -1) {
									d.text = d.text.replace(p, e[p]);
									for (var m = 0; m < h.length; m++) {
										var _ = h[m].attributes;
										if (_.Name === p) {
											if (s.mixin(a.features[o], {
													_associatedStop: h[this.returnToStart && m === h.length - 1 ? 0 : m],
													_associatedStopWithReturnToStart: h[m]
												}), !_.ArriveTime && !_.ArriveTimeUTC && d.ETA && d.arriveTimeUTC) {
												_.ArriveTime = d.ETA,
												_.ArriveTimeUTC = d.arriveTimeUTC;
												var g = 0;
												(0 == o && "esriDMTDepart" === d.maneuverType || o > 0 && "esriDMTStop" === d.maneuverType) && (g = this._convertCostValue(d.time, this._getDirectionsTimeAttribute().units, "milliseconds")),
												_.DepartTime = _.ArriveTime + g,
												_.DepartTimeUTC = _.ArriveTimeUTC + g
											}
											break
										}
									}
								}
					}
					for (o = 0; o < h.length; o++)
						this._returnToStartStop && h[o].attributes.Name === this._returnToStartStop.feature.attributes.Name && (this._returnToStartStop._resultsStopIndex = o), h[o].attributes.Name = e[h[o].attributes.Name];
					this._directionsPostprocessing(a),
					this.traffic && this._updateMapTimeExtent()
				}
				return this._showRoute(t).always(s.hitch(this, function () {
						this._incrementalSolveStopRange = null,
						r.resolve(t)
					})),
				r.promise
			},
			_directionsPostprocessing: function (t) {
				var e,
				i,
				r = function (e, s) {
					var i = t.features[e]._associatedFeaturesWithWaypoints || [];
					i.push(t.featuresWithWaypoints[s]),
					t.features[e]._associatedFeaturesWithWaypoints = i,
					t.featuresWithWaypoints[s]._associatedFeatureNoWaypoints = t.features[e]
				},
				o = s.hitch(this, function (t) {
						return "<div class='" + this.theme + "'><table class='esriRoutesTooltip'>" + this._renderDirectionsItemTR(t._associatedFeatureNoWaypoints) + "</table></div>"
					});
				for (t.featuresWithWaypoints = [], e = 0; e < t.features.length; e++) {
					var n = new E(t.features[e].toJson());
					n.setInfoTemplate(new W({
							title: this._i18n.widgets.directions.maneuver,
							content: o
						})),
					t.featuresWithWaypoints.push(n),
					t.featuresWithWaypoints[e]._associatedStop = t.features[e]._associatedStop,
					t.featuresWithWaypoints[e]._associatedStopWithReturnToStart = t.features[e]._associatedStopWithReturnToStart
				}
				for (t.stringsWithWaypoints = s.mixin([], t.strings), t.eventsWithWaypoints = s.mixin([], t.events), e = 0, i = 0; e < t.features.length; )
					if (t.features[e]._associatedStop && t.features[e]._associatedStop.attributes.Name === this._waypointName)
						if (t.features.splice(e, e ? 2 : 1), t.strings.splice(e, e ? 2 : 1), t.events.splice(e, e ? 2 : 1), e && t.features[e] && "esriDMTStraight" === t.features[e].attributes.maneuverType) {
							var a = t.features.splice(e, 1)[0];
							t.strings[e - 1] = (t.strings[e - 1] || []).concat(t.strings.splice(e, 1)[0] || []),
							t.strings[e - 1].length || (t.strings[e - 1] = void 0),
							t.events[e - 1] = (t.events[e - 1] || []).concat(t.events.splice(e, 1)[0] || []),
							t.events[e - 1].length || (t.events[e - 1] = void 0),
							t.features[e - 1].attributes.length += a.attributes.length,
							t.features[e - 1].attributes.time += a.attributes.time,
							t.features[e - 1].geometry.paths[0] = t.features[e - 1].geometry.paths[0].concat(a.geometry.paths[0]),
							r(e - 1, i++),
							r(e - 1, i++),
							r(e - 1, i++)
						} else
							e && r(e - 1, i++), e < t.features.length && r(e, i++);
					else
						r(e, i++), e++
			},
			_boldText: function (t, e) {
				var s = t;
				try {
					var i = new RegExp("[^<strong>&nbsp;]" + e.replace(/(\||\$|\^|\(|\)|\[|\]|\{|\}|\/|\.|\+|\*|\?|\?)/g, "\\$1") + "[^&nbsp;</strong>]", "g");
					s = (" " + s + " ").replace(i, "<strong>&nbsp;" + e + "&nbsp;</strong>"),
					s = s.trim()
				} catch (r) {}
				return s
			},
			_clearStopGraphics: function () {
				if (this.stopGraphics && this.stopGraphics.length)
					for (var t = 0; t < this.stopGraphics.length; t++)
						this._stopLayer.remove(this.stopGraphics[t]), this._stopLayer.remove(this.textGraphics[t]);
				this.set("stopGraphics", []),
				this.set("textGraphics", [])
			},
			_updateMapTimeExtent: function () {
				if (this.map) {
					var t = this.directions && this.directions.features[0] && this.directions.features[0].attributes || {},
					e = (new Date).getTime(),
					s = "none" == this.startTime ? e : t.arriveTimeUTC || e;
					this._useTrafficItemNode.title = "none" !== this.startTime && t.arriveTimeUTC ? C.widgets.directions.trafficLabelDepartAt + ": " + this._toSpatiallyLocalTimeString(t.arriveTimeUTC, t.ETA) : "",
					this._myTimeExtentUpdate = !0;
					var i = new Date(s);
					this.map.setTimeExtent(new k(i, i))
				}
			},
			_restoreMapTimeExtent: function () {
				this.map && (this._myTimeExtentUpdate = !0, this.map.setTimeExtent(this._externalTimeExtent))
			},
			_clearRouteGraphics: function () {
				for (var t = this.displayedRouteGraphics, e = this.displayedManeuverPointGraphics, s = this._routeLayer, i = this._waypointsEventLayer, o = 0, n = 0, a = this._incrementalSolveStopRange ? this._incrementalSolveStopRange : {
						start: 0,
						end: this.stops ? this.stops.length : -1
					}; t && n < t.length; ) {
					if (t[n]._associatedStop)
						for (var h = 0; h < this.stops.length; h++)
							if (this.stops[h].feature === t[n]._associatedStop) {
								o = h;
								break
							}
					o >= a.start && o < a.end || a.start >= a.end && (o >= a.start || o < a.end) ? (s.remove(t[n]), t.splice(n, 1)) : n++
				}
				s.remove(this._incrementalRouteSegment),
				this.set("displayedRouteGraphics", t ? t : []),
				e && e.length && r.forEach(e, function (t) {
					i.remove(t)
				}),
				this.set("displayedManeuverPointGraphics", []),
				this._waypointsEventLayer.clear(),
				this.unhighlightSegment(!0)
			},
			_clearInfoWindow: function () {
				var t = this.get("map").infoWindow;
				t && (t.hide(), t.clearFeatures())
			},
			_clearDisplayBeforeSolve: function () {
				this._toggleSaveMenu(!1),
				this._clearInfoWindow(),
				this._clearResultsHTML()
			},
			_clearDisplayAfterSolve: function () {
				this._clearStopGraphics(),
				this._clearRouteGraphics(),
				this._clearBarriersGraphics(),
				this.clearMessages()
			},
			_getLetter: function (t) {
				var e = this.alphabet,
				i = "",
				r = function (t) {
					var s = "";
					return "0123456789" === e || "1234567890" === e ? s = String(t + 1) : (t = t || 0, t >= e.length && (s = r(Math.floor(t / e.length) - 1), t %= e.length), s += e[t]),
					s
				},
				o = t instanceof E ? s.hitch(this, function () {
						for (var e = this.get("returnToStart") ? 0 : -1, s = 0; s < this.stops.length; s++)
							if (this.stops[s].feature === t) {
								e = s;
								break
							}
						return e
					})() : t;
				if (o > -1 && e && e.length) {
					e instanceof Array && (e = e.toString().replace(/,/g, ""));
					for (var n = -1, a = 0; o >= a; a++)
						n += this._isStopAWaypoint(this.stops[a]) ? 0 : 1;
					i = r(n)
				}
				return i
			},
			_solveAndZoom: function (t) {
				if (this.autoSolve)
					return this._getDirections().then(s.hitch(this, function () {
							t || this.zoomToFullRoute()
						}));
				var e = new P;
				return e.resolve(),
				e.promise
			},
			_setupEvents: function () {
				this._onEvents.push(g(this.domNode, "[data-blur-on-click]:click", function () {
						this.blur()
					})),
				this._onEvents.push(g(this._dndNode, "[data-reverse-stops]:click, [data-reverse-stops]:keydown", s.hitch(this, function (t) {
							t && ("click" === t.type || "keydown" === t.type && t.keyCode === m.ENTER) && this.modifyStopSequence()
						}))),
				this._onEvents.push(g(this._printButton, "click, keydown", s.hitch(this, function (t) {
							t && ("click" === t.type || "keydown" === t.type && t.keyCode === m.ENTER) && this._printDirections()
						}))),
				this._onEvents.push(g(this._resultsNode, "[data-full-route]:click, [data-full-route]:keydown", s.hitch(this, function (t) {
							t && ("click" === t.type || "keydown" === t.type && t.keyCode === m.ENTER) && this.zoomToFullRoute()
						}))),
				this._onEvents.push(g(this._dndNode, "[data-remove]:click, [data-remove]:keydown", s.hitch(this, function (t) {
							if (t && ("click" === t.type || "keydown" === t.type && t.keyCode === m.ENTER)) {
								var e = b("[data-remove]", this._dndNode),
								s = r.indexOf(e, t.target);
								this.removeStop(s)
							}
						}))),
				this._onEvents.push(g(this._dndNode, "[data-center-at]:click, [data-center-at]:keydown", s.hitch(this, function (t) {
							if (t && ("click" === t.type || "keydown" === t.type && t.keyCode === m.ENTER)) {
								var e = b("[data-center-at]", this._dndNode),
								s = r.indexOf(e, t.target);
								this.stops[s] && this.stops[s].feature && this.stops[s].feature.geometry && this.map.centerAndZoom(this.stops[s].feature.geometry)
							}
						}))),
				this._onEvents.push(g(this.map, "zoom-end", s.hitch(this, function () {
							var t = this._segmentGraphics;
							t && t[0] && void 0 !== t[0].attributes._index && this.highlightSegment(t[0].attributes._index, !0)
						}))),
				this._onEvents.push(g(this._dnd, "Drop", s.hitch(this, function () {
							this._dnd.sync(),
							this.set("optimalRoute", !1);
							var t,
							e,
							s = !1,
							i = [],
							o = this._dnd.getAllNodes();
							for (t = 0; t < this.geocoders.length; t++)
								if (e = r.indexOf(o, this.geocoders[t]._tr), e > -1 && t !== e && t !== e - 1) {
									for (; e > 0 && this._isStopAWaypoint(this.stops[e]); )
										e--;
									s = !0;
									break
								}
							s && (i = this._markWPsForRemovalAfterUserChangedStopSequence(t, e)),
							this._sortGeocoders(),
							this.setListIcons(),
							this._removeSomeWaypoints(i),
							this.stops[e].name && this.getDirections()
						}))),
				this._onEvents.push(g(this._dnd, "DndStart", s.hitch(this, function () {
							var t = b("body")[0];
							T.add(t, this._css.dndDragBodyClass),
							this._removeLocateButtonVisibilityEvents()
						}))),
				this._onEvents.push(g(this._dnd, "DndDrop, DndCancel", s.hitch(this, function () {
							var t = b("body")[0];
							T.remove(t, this._css.dndDragBodyClass),
							this._setLocateButtonVisibilityEvents()
						})));
				var t = this._handle,
				e = s.hitch(this, function (e) {
						var i = s.hitch(this, function (s, i, r, o, n) {
								var a = t._isShown();
								if (t.setGeometry(s), t._tooltip.style.left = e.screenPoint.x + "px", t._tooltip.style.top = e.screenPoint.y + "px", !a || t._index === r && t.attributes.isWaypoint === o || (t._remove(), a = !1), t.setSymbol(i), t._index = r, t._isStopIcon = t._index === Math.floor(t._index), t.attributes.isWaypoint = o, t._isStopIcon && (t.attributes.address = this.stopGraphics[r].attributes.address, t.setInfoTemplate(this.stopGraphics[r].infoTemplate)), this.canModifyWaypoints || t._isStopIcon) {
									if (!a) {
										this._stopLayer.add(t);
										var h = t.getDojoShape();
										h && h[o ? "moveToBack" : "moveToFront"].call(h)
									}
									this.editToolbar.activate(ut.MOVE, t)
								}
								var c = o ? t._isStopIcon ? C.widgets.directions.dragWaypoint : this.canModifyWaypoints ? C.widgets.directions.dragRoute : "" : C.widgets.directions.dragStop;
								t._showTooltip(n || c)
							});
						if (this.unhighlightSegment(), this.mapClickActive && this.dragging && !this._solveInProgress && !this._moveInProgress && !t._solveTimeout)
							if (clearTimeout(t._removeTimeout), (e.graphic._isStopIcon || e.graphic._isStopLabel) && !e.graphic.attributes.isWaypoint && this.canModifyStops || e.graphic._isStopIcon && e.graphic.attributes.isWaypoint && this.canModifyWaypoints) {
								var o = e.graphic._isStopLabel ? this.stopGraphics[e.graphic._index] : e.graphic;
								i(o.geometry, this._getStopSymbol(this.stops[o._index], !0), o._index, o.attributes.isWaypoint === !0),
								this._stopLayer.remove(this.stopGraphics[o._index]),
								this._stopLayer.remove(this.textGraphics[o._index])
							} else (e.graphic._isSnapFeature || e.graphic._isHandle && !e.graphic._isStopIcon) && (this._snappingManager || (this._snappingManager = this.map.snappingManager), this._snappingManager.getSnappingPoint(e.screenPoint).then(s.hitch(this, function (s) {
											if (!this.maxStopsReached && !this._moveInProgress && s) {
												for (var o = this.displayedManeuverPointGraphics, n = null, a = 0; a < o.length; a++)
													if (o[a].geometry.x === s.x && o[a].geometry.y === s.y) {
														n = o[a]._directionsFeature;
														var h = r.indexOf(this.directions.features, n);
														h > -1 && this.highlightSegment(h);
														break
													}
												i(s, e.graphic._isHandle ? t.symbol : this.waypointSymbol, e.graphic._isHandle ? t._index : e.graphic._nextStopIndex, e.graphic._isHandle ? t.attributes.isWaypoint : !0, n)
											}
										})))
					}),
				i = s.hitch(this, function () {
						clearTimeout(t._removeTimeout),
						t._removeTimeout = setTimeout(t._remove, 100),
						this.unhighlightSegment()
					}),
				o = s.hitch(this, function (t) {
						if (this.barrierToolActive) {
							var e = t.graphic,
							s = this.routeParams,
							i = s.barriers ? r.indexOf(s.barriers.features, e) : -1,
							o = s.polygonBarriers ? r.indexOf(s.polygonBarriers.features, e) : -1,
							n = s.polylineBarriers ? r.indexOf(s.polylineBarriers.features, e) : -1;
							i > -1 && s.barriers.features.splice(i, 1),
							o > -1 && s.polygonBarriers.features.splice(o, 1),
							n > -1 && s.polylineBarriers.features.splice(n, 1),
							this._barriersLayer.remove(e),
							this._polygonBarriersLayer.remove(e),
							this._polylineBarriersLayer.remove(e),
							this._clearStopsStatusAttr(),
							this.getDirections(!0)
						}
					});
				this._onEvents.push(this._waypointsEventLayer.on("mouse-move", e)),
				this._onEvents.push(this._waypointsEventLayer.on("mouse-out", i)),
				this._onEvents.push(this._stopLayer.on("mouse-move", e)),
				this._onEvents.push(this._stopLayer.on("mouse-out", i)),
				this._onEvents.push(this._barriersLayer.on("click", o)),
				this._onEvents.push(this._polylineBarriersLayer.on("click", o)),
				this._onEvents.push(this._polygonBarriersLayer.on("click", o)),
				this._editToolbarEvents(),
				this._watchEvents.push(this.watch("theme", this._updateThemeWatch)),
				this._watchEvents.push(this.watch("canModifyStops", this._updateCanModifyStops)),
				this._watchEvents.push(this.watch("canModifyWaypoints", this._updateCanAddWaypoints)),
				this._watchEvents.push(this.watch("showReturnToStartOption", this._optionsMenu)),
				this._watchEvents.push(this.watch("showOptimalRouteOption", this._optionsMenu)),
				this._watchEvents.push(this.watch("returnToStart", this._setMenuNodeValues)),
				this._watchEvents.push(this.watch("optimalRoute", this._setMenuNodeValues)),
				this._watchEvents.push(this.watch("startTime", this._setStartTime)),
				this._watchEvents.push(this.watch("traffic", this._setMenuNodeValues)),
				this._watchEvents.push(this.watch("trafficLayer", this._trafficLayerUpdate)),
				this._watchEvents.push(this.watch("routeTaskUrl", s.hitch(this, function () {
							this._createRouteTask(),
							this._setTrafficOptions()
						}))),
				this._watchEvents.push(this.watch("printTaskUrl", s.hitch(this, function () {
							this._createPrintTask()
						}))),
				this._watchEvents.push(this.watch("geometryTaskUrl", s.hitch(this, function () {
							this._createGeometryTask()
						}))),
				this._watchEvents.push(this.watch("routeParams", s.hitch(this, function () {
							this._createRouteParams(),
							this._setDefaultUnits()
						}))),
				this._watchEvents.push(this.watch("searchOptions", s.hitch(this, function () {
							this._setSearchOptions(),
							this._createGlobalGeocoder();
							var t = this.get("searchOptions").sources;
							if (t)
								for (var e = 0; e < this.geocoders.length; e++)
									this.geocoders[e].set("sources", t)
						}))),
				this._watchEvents.push(this.watch("showReverseStopsButton", this.setListIcons)),
				this._watchEvents.push(this.watch("editToolbar", this._editToolbarEvents)),
				this._watchEvents.push(this.watch("showTravelModesOption", this._showTravelModesOption)),
				this._watchEvents.push(this.watch("showMilesKilometersOption", this._showMilesKilometersOption)),
				this._watchEvents.push(this.watch("showClearButton", this._showClearButton)),
				this._watchEvents.push(this.watch("directionsLengthUnits", this.setDirectionsLengthUnits)),
				this._watchEvents.push(this.watch("directionsLanguage", this.setDirectionsLanguage)),
				this._watchEvents.push(this.watch("mapClickActive", this._activate)),
				this._watchEvents.push(this.watch("barrierToolActive", this._activateBarrierTool)),
				this._watchEvents.push(this.watch("showActivateButton", this._showActivateButton)),
				this._watchEvents.push(this.watch("showBarriersButton", this._showBarriersButton)),
				this._watchEvents.push(this.watch("showPrintPage", function () {
						S.set(this._printButton, "display", this.showPrintPage ? "inline-block" : "none")
					})),
				this._watchEvents.push(this.watch("showSaveButton", function () {
						S.set(this._saveMenuButton, "display", this.showSaveButton && this.owningSystemUrl ? "inline-block" : "none")
					}))
			},
			_editToolbarEvents: function () {
				var t = s.hitch(this, function (t) {
						var e = "",
						s = t.routeResults ? t.routeResults[0] : null;
						if (s && s.route) {
							var i = s.route.attributes,
							r = this.routeParams.travelMode;
							if (r) {
								e += "<b>" + r.name + "</b> " + C.widgets.directions.toNearbyStops + ": ";
								var o = void 0 !== i["Total_" + r.timeAttributeName] ? this._formatTime(i["Total_" + r.timeAttributeName], !1, this._getCostAttribute(r.timeAttributeName).units) : "";
								e += (void 0 !== i["Total_" + r.distanceAttributeName] ? this._formatDistance(i["Total_" + r.distanceAttributeName], !0, this._getCostAttribute(r.distanceAttributeName).units) : "") + (o ? " &middot; " + o : "")
							} else
								e = this._formatArbitraryCostsForRouteTooltip(i)
						}
						this._handle._showTooltip(e)
					}),
				e = s.hitch(this, function (i, r) {
						var o = new P,
						n = this._handle,
						a = this.map.toScreen(n._origPoint).offset(i.transform.dx + n.symbol.xoffset, i.transform.dy + n.symbol.yoffset);
						if (i.origMapPoint || (i.origMapPoint = this.map.toMap(a)), S.set(n._tooltip, "left", a.x + "px"), S.set(n._tooltip, "top", a.y + "px"), clearTimeout(n._solveTimeout), this._solveInProgress || !this._requestQueueTail.isFulfilled())
							n._solveTimeout = setTimeout(function () {
									e(i, r).always(o.resolve)
								}, 100);
						else if (n._isStopIcon) {
							var h = i.graphic._index,
							c = this.stops[h],
							u = c ? {
								name: c.name,
								extent: c.extent,
								feature: new E(c.feature.toJson())
							}
							 : null;
							u ? (u.feature.setGeometry(i.origMapPoint), this._modifiedWaypointIndex = this._isStopAWaypoint(this.stops[h]) ? h : null, this._incrementalSolveStopRange = {
									start: this.returnToStart ? (this.stops.length + h - 1) % this.stops.length : Math.max(0, h - 1),
									end: this.returnToStart ? (h + 1) % this.stops.length : Math.min(this.stops.length - 1, h + 1)
								}, this.updateStop(u, h, r).always(s.hitch(this, function (e) {
										n._solveTimeout = null,
										e.routeResults && t(e),
										o.resolve()
									}))) : o.resolve()
						} else
							o.resolve();
						return o.promise
					});
				this._onEvents.push(g(this.editToolbar, "graphic-click", s.hitch(this, function (t) {
							if (t.graphic.attributes.isWaypoint)
								!t.graphic._isStopIcon || this._moveInProgress || this._solveInProgress || (this._handle._remove(), this._moveInProgress = !0, this.removeStop(t.graphic._index, !0).always(s.hitch(this, function () {
											this._moveInProgress = !1
										})));
							else {
								var e = this.get("map").infoWindow;
								e && (e.setFeatures([t.graphic]), e.show(t.graphic.geometry))
							}
						}))),
				this._onEvents.push(g(this.editToolbar, "graphic-move-start", s.hitch(this, function (t) {
							this._blurGeocoders(),
							this._moveInProgress = !0,
							this._removeEmptyStops(),
							this.routeParams.returnDirections = !1,
							this.routeParams.returnRoutes = !0;
							var e = t.graphic;
							e._origPoint = new Z(e.geometry.toJson()),
							e._maxDeviation = 0,
							e._solveHasHappened = !1,
							this.map.disableMapNavigation()
						}))),
				this._onEvents.push(g(this.editToolbar, "graphic-move-stop", s.hitch(this, function (t) {
							if (this.map.enableMapNavigation(), this.routeParams.returnDirections = !0, this.routeParams.returnRoutes = !1, this._handle._isStopIcon)
								if (this._handle._solveHasHappened)
									if (this._handle.attributes.isWaypoint)
										e(t, !0).always(s.hitch(this, function () {
												this._moveInProgress = !1,
												this._handle._isStopIcon = !0,
												this._handle._remove(),
												this._showLoadingSpinner()
											}));
									else {
										clearTimeout(this._handle._solveTimeout),
										this._handle._solveTimeout = null;
										var i = this.stops[t.graphic._index],
										r = i && i.feature && i.feature.attributes && i.feature.attributes.Name,
										o = r === this._userDefinedStopName,
										n = i && i.name;
										this._reverseGeocode(new E(t.graphic.toJson())).then(s.hitch(this, function (e) {
												o && (e.name = n, e.feature.attributes.Name = this._userDefinedStopName),
												this._setReverseGeocode(e, e.feature.geometry, t.graphic._index).always(s.hitch(this, function () {
														this._moveInProgress = !1,
														this._handle._remove(),
														this._showLoadingSpinner()
													}))
											}))
									}
								else
									this._moveInProgress = !1;
							else
								this._moveInProgress = !1
						}))),
				this._onEvents.push(g(this.editToolbar, "graphic-move", s.hitch(this, function (t) {
							var s = this._handle,
							i = t.transform;
							if (i.dx && i.dy && (t.graphic._maxDeviation = Math.max(s._maxDeviation, Math.sqrt(i.dx * i.dx + i.dy * i.dy))), s._maxDeviation > 10)
								if (s._solveHasHappened = !0, t.graphic === s && s.attributes.isWaypoint && !s._isStopIcon)
									this.set("optimalRoute", !1), s._index += .5, s._isStopIcon = !0, t.graphic._stopIndex = s._index, this._incrementalSolveStopRange = {
										start: this.returnToStart ? (this.stops.length + s._index - 1) % this.stops.length : Math.max(0, s._index - 1),
										end: this.returnToStart ? (s._index + 1) % (this.stops.length + 1) : Math.min(this.stops.length, s._index + 1)
									},
							this.addStop({
								name: this._waypointName,
								feature: new E(s.geometry, s.symbol, {
									isWaypoint: !0,
									CurbApproach: 3
								})
							}, t.graphic._stopIndex);
							else {
								var r = t.graphic.getDojoShape();
								r && r.moveToFront(),
								e(t)
							}
						})))
			},
			_isStopAWaypoint: function (t) {
				return t && t.feature && t.feature.attributes && t.feature.attributes.isWaypoint
			},
			_getStopCount: function () {
				var t,
				e = 0;
				for (t = 0; t < this.stops.length; t++)
					e += !this._isStopAWaypoint(this.stops[t]) && this.stops[t].name ? 1 : 0;
				return e
			},
			_getWaypointCount: function () {
				var t,
				e = 0;
				for (t = 0; t < this.stops.length; t++)
					e += this._isStopAWaypoint(this.stops[t]) ? 1 : 0;
				return e
			},
			_decorateUngeocodedStop: function (t) {
				var e = new P,
				i = function (s, i) {
					e.resolve({
						name: void 0 === s ? C.widgets.directions.unlocatedStop : s.toFixed(6) + " " + i.toFixed(6),
						feature: t
					})
				};
				if (t.geometry)
					if (t.geometry.spatialReference && 4326 !== t.geometry.spatialReference.wkid)
						if (this.map && this.map.spatialReference && this.map.spatialReference.isWebMercator()) {
							var r = F.xyToLngLat(t.geometry.x, t.geometry.y);
							i(r[0], r[1])
						} else if (this._geometryService) {
							var o = new pt;
							o.outSR = new U(4326),
							o.geometries = [t.geometry],
							this._geometryService.project(o).then(s.hitch(this, function (e) {
									e && e.length ? i(e[0].x, e[0].y) : i(t.geometry.x, t.geometry.y)
								}), s.hitch(this, function () {
									i()
								}))
						} else
							i(t.geometry.x, t.geometry.y);
					else
						i(t.geometry.x, t.geometry.y);
				else
					i();
				return e.promise
			},
			_trafficLayerUpdate: function () {
				var t = arguments[1],
				e = arguments[2],
				s = this.get("map");
				t && this._trafficLayerAdded && (s.removeLayer(t), this._trafficLayerAdded = !1),
				e && this.get("traffic") && !this._trafficLayerAdded && (s.addLayer(e), e.show(), this._trafficLayerAdded = !0)
			},
			_routeTaskError: function (t) {
				var e = C.widgets.directions.error.routeTask,
				s = t.details,
				i = function (t) {
					return res = t.match(/(\d+)/),
					res ? ": " + res[0] : "."
				};
				s && 1 === s.length && ("The distance between any inputs must be less than 50 miles (80 kilometers) when walking." === s[0] ? e = C.widgets.directions.error.maxWalkingDistance : "Driving a truck is currently not supported outside of North America and Central America." === s[0] ? e = C.widgets.directions.error.nonNAmTruckingMode : 0 === s[0].indexOf("The number of input locations loaded into Barriers") ? e = C.widgets.directions.error.tooManyBarriers + i(s[0]) : 0 === s[0].indexOf("The number of input locations loaded into PolygonBarriers") ? e = C.widgets.directions.error.tooManyPolygonBarriers + i(s[0]) : 0 === s[0].indexOf("The number of input locations loaded into PolylineBarriers") && (e = C.widgets.directions.error.tooManyPolylineBarriers + i(s[0]))),
				this._showMessage(e),
				this.onDirectionsFinish(t)
			},
			_showMessage: function (t, e) {
				var s = "";
				if (this.messages.push({
						msg: t,
						error: !e
					}), this.messages.length) {
					s += "<ul>";
					for (var i = 0; i < this.messages.length; i++)
						s += '<li class="' + (this.messages[i].error ? this._css.routesErrorClass : this._css.routesInfoClass) + '">' + this.messages[i].msg + "</li>";
					s += "</ul>"
				}
				this._msgNode && (this._msgNode.innerHTML = s),
				e || this.onError(t)
			},
			_isTimeUnits: function (t) {
				return "milliseconds" === t || "esriNAUSeconds" === t || "esriNAUMinutes" === t || "esriNAUHours" === t || "esriNAUDays" === t
			},
			_getImpedanceAttribute: function () {
				var t = this.routeParams && this.routeParams.travelMode && this.routeParams.travelMode.impedanceAttributeName || this.routeParams.impedanceAttribute || this.serviceDescription.impedance;
				return this._getCostAttribute(t)
			},
			_getDirectionsTimeAttribute: function () {
				var t = this.routeParams && this.routeParams.travelMode && this.routeParams.travelMode.timeAttributeName || this.routeParams.directionsTimeAttribute || this.serviceDescription.directionsTimeAttribute;
				return this._getCostAttribute(t)
			},
			_getTimeNeutralAttribute: function () {
				var t = this._getDirectionsTimeAttribute(),
				e = (t || {}).timeNeutralAttributeName;
				return this._getCostAttribute(e)
			},
			_getCostAttribute: function (t) {
				for (var e, s = this.serviceDescription && this.serviceDescription.networkDataset.networkAttributes || [], i = 0; i < s.length; i++)
					if (s[i].name === t && "esriNAUTCost" === s[i].usageType) {
						e = s[i];
						break
					}
				return e
			},
			_convertCostValue: function (t, e, s) {
				var i = this._isTimeUnits(e),
				r = this._isTimeUnits(s),
				o = i ? this._toMinutes(t, e) : this._toMeters(t, e);
				return i === r ? r ? this._fromMinutes(o, s) : this._fromMeters(o, s) : t
			},
			_toMinutes: function (t, e, s) {
				switch (t = t || 0, e) {
				case "milliseconds":
					t /= Math.pow(6e4, s ? -1 : 1);
					break;
				case "esriNAUSeconds":
					t /= Math.pow(60, s ? -1 : 1);
					break;
				case "esriNAUHours":
					t *= Math.pow(60, s ? -1 : 1);
					break;
				case "esriNAUDays":
					t *= Math.pow(1440, s ? -1 : 1)
				}
				return t
			},
			_fromMinutes: function (t, e) {
				return this._toMinutes(t, e, !0)
			},
			_toMeters: function (t, e, s) {
				switch (t = t || 0, (e || "").replace("esriNAU", "esri")) {
				case "esriInches":
					t *= Math.pow(.0254, s ? -1 : 1);
					break;
				case "esriFeet":
					t *= Math.pow(.3048, s ? -1 : 1);
					break;
				case "esriYards":
					t *= Math.pow(.9144, s ? -1 : 1);
					break;
				case "esriMiles":
					t *= Math.pow(1609.344, s ? -1 : 1);
					break;
				case "esriNauticalMiles":
					t *= Math.pow(1851.995396854, s ? -1 : 1);
					break;
				case "esriMillimeters":
					t /= Math.pow(1e3, s ? -1 : 1);
					break;
				case "esriCentimeters":
					t /= Math.pow(100, s ? -1 : 1);
					break;
				case "esriKilometers":
					t *= Math.pow(1e3, s ? -1 : 1);
					break;
				case "esriDecimeters":
					t /= Math.pow(10, s ? -1 : 1)
				}
				return t
			},
			_fromMeters: function (t, e) {
				return this._toMeters(t, e, !0)
			},
			_createRouteTask: function () {
				var t = new P;
				return this.set("routeTask", new it(this.get("routeTaskUrl"))),
				this._createRouteParams(),
				this.routeTask.getServiceDescription(this.travelModesServiceUrl, this.doNotFetchTravelModesFromOwningSystem).then(s.hitch(this, function (e) {
						e.networkDataset ? (this.set("serviceDescription", e), this.set("maxStops", parseInt(this.userOptions.maxStops || e.serviceLimits && e.serviceLimits.Route_MaxStops || this.defaults.maxStops)), this.defaults.portalUrl ? (this.owningSystemUrl = this.defaults.portalUrl, t.resolve()) : this.routeTask.getOwningSystemUrl().then(s.hitch(this, function (e) {
									this.owningSystemUrl = e,
									t.resolve()
								}), t.reject)) : (this._showMessage(C.widgets.directions.error.cantFindRouteServiceDescription), t.reject(new Error(C.widgets.directions.error.cantFindRouteServiceDescription)))
					}), s.hitch(this, function () {
						this._showMessage(C.widgets.directions.error.cantFindRouteServiceDescription),
						t.reject(new Error(C.widgets.directions.error.cantFindRouteServiceDescription)),
						this.mapClickActive = !1,
						this._activate()
					})),
				t.promise
			},
			_createSearchSourceDDL: function () {
				if (this._globalGeocoder && this._globalGeocoder.sources && this._globalGeocoder.sources.length > 1) {
					var t = s.hitch(this, function (t) {
							this._searchSourceSelector.domNode.blur(),
							this._globalGeocoder.set("activeSourceIndex", t);
							for (var e = 0; e < this.geocoders.length; e++)
								this.geocoders[e].set("activeSourceIndex", t)
						});
					if (!this._searchSourceSelector) {
						for (var e = [{
									value: "all",
									label: C.widgets.Search.main.all,
									selected: !0
								}
							], i = 0; i < this._globalGeocoder.sources.length; i++)
							e.push({
								value: String(i),
								label: this._globalGeocoder.sources[i].name
							});
						this._searchSourceSelector = new h({
								className: "esriSearchSourcesDDL",
								style: "width:100%;",
								options: e
							}, this._searchSourceSelectorContainer),
						this._searchSourceSelector.startup(),
						this._searchSourceSelector.on("change", t),
						this._searchSourceSelector.domNode.style.width = ""
					}
					t("all")
				} else
					this._globalGeocoder.sources.length < 2 && (this._searchSourceContainerNode.style.display = "none")
			},
			_createTravelModesDDL: function () {
				this._travelModeSelector || (this._travelModeSelector = new h({
							className: "esriTravelModesDDL",
							style: "width:100%;"
						}, this._travelModeSelectorContainer), this._travelModeSelector.startup(), this._travelModeSelector._interractive = !0, this._travelModeSelector.on("change", s.hitch(this, function (t) {
							this._travelModeSelector._interractive ? this._enqueue(function () {
								return this._setTravelMode(t).always(s.hitch(this, function () {
										this._travelModeSelector._interractive = !0
									}))
							}) : this._travelModeSelector._interractive = !0
						})))
			},
			_setupTravelModes: function () {
				var t = this.get("serviceDescription"),
				e = t.supportedTravelModes,
				s = new P;
				if (e && e.length && this._travelModeSelector) {
					for (var i = e[0].name, r = [], o = 0; o < e.length; o++) {
						for (var n = "AUTOMOBILE" === e[o].type ? "Driving" : "TRUCK" === e[o].type ? "Trucking" : "WALK" === e[o].type ? "Walking" : "Other", a = "", h = t.networkDataset.networkAttributes, c = 0; c < h.length; c++) {
							var u = h[c];
							if (u.name === e[o].impedanceAttributeName) {
								"esriNAUCentimeters" === u.units || "esriNAUDecimalDegrees" === u.units || "esriNAUDecimeters" === u.units || "esriNAUFeet" === u.units || "esriNAUInches" === u.units || "esriNAUKilometers" === u.units || "esriNAUMeters" === u.units || "esriNAUMiles" === u.units || "esriNAUMillimeters" === u.units || "esriNAUNauticalMiles" === u.units || "esriNAUYards" === u.units ? a = "Distance" : ("esriNAUDays" === u.units || "esriNAUHours" === u.units || "esriNAUMinutes" === u.units || "esriNAUSeconds" === u.units) && (a = "Time");
								break
							}
						}
						r.push({
							id: e[o].name,
							label: '<div class="esriTravelModesDirectionsIcon esriTravelModesType' + n + a + '">&nbsp;</div><div class="esriTravelModesTypeName" title="' + e[o].description + '">' + e[o].name + "</div>"
						}),
						!t.defaultTravelMode || e[o].id !== t.defaultTravelMode && e[o].itemId !== t.defaultTravelMode || (i = e[o].name)
					}
					this._showTravelModesOption(),
					this._travelModeSelector.setStore(new p({
							objectStore: new d({
								data: r
							})
						})),
					this._travelModeSelector._interractive = !1,
					this._travelModeSelector.setValue(i),
					this._setTravelMode(i).always(s.resolve)
				} else
					this._checkStartTimeUIAvailability(), this.set("showTravelModesOption", !1), s.resolve();
				return s.promise
			},
			_createPrintTask: function () {
				this.printTaskUrl = this._usingAGOL() ? this.printTaskUrl : this.defaults.printTaskUrl,
				this._printService = this.printTaskUrl ? new at(this.printTaskUrl, {
						async: !1
					}) : null;
				var t = new ct;
				t.exportOptions = {
					width: 670,
					height: 750,
					dpi: 96
				},
				t.format = "PNG32",
				t.layout = "MAP_ONLY",
				t.preserveScale = !1,
				t.showAttribution = !1;
				var e = new ht;
				e.map = this.map,
				e.outSpatialReference = this.map.spatialReference,
				e.template = t,
				this._printParams = e
			},
			_createGeometryTask: function () {
				this._geometryService = null,
				this._usingAGOL() ? this._geometryService = new ot(this.geometryTaskUrl) : (this._geometryService = this.defaults.geometryTaskUrl ? new ot(this.defaults.geometryTaskUrl) : dt.defaults.geometryService, this.geometryTaskUrl = this._geometryService ? this._geometryService.url : null)
			},
			_showTravelModesOption: function () {
				var t = this.get("serviceDescription"),
				e = this.showTravelModesOption && t && t.supportedTravelModes && t.supportedTravelModes.length && !this.doNotFetchTravelModesFromOwningSystem;
				S.set(this._travelModeContainerNode, "display", e ? "block" : "none")
			},
			_showMilesKilometersOption: function () {
				S.set(this._agolDistanceUnitsNode, "display", this.showMilesKilometersOption ? "block" : "none")
			},
			_showClearButton: function () {
				S.set(this._clearDirectionsButtonNode, "display", this.showClearButton ? "inline-block" : "none")
			},
			_showActivateButton: function () {
				S.set(this._activateButtonNode, "display", this.showActivateButton ? "inline-block" : "none"),
				this.showActivateButton || this.deactivate()
			},
			_showBarriersButton: function () {
				S.set(this._lineBarrierButtonNode, "display", this.showBarriersButton ? "inline-block" : "none"),
				this.showBarriersButton || this.deactivateBarrierTool()
			},
			_createRouteParams: function () {
				var t = {
					outputGeometryPrecision: 0,
					outputGeometryPrecisionUnits: "esriMeters",
					restrictUTurns: "esriNFSBAtDeadEndsOnly"
				},
				e = {
					returnDirections: !0,
					returnRoutes: !1,
					outputLines: "esriNAOutputLineTrueShape",
					preserveFirstStop: !0,
					preserveLastStop: !0,
					directionsOutputType: "complete",
					stops: new st,
					ignoreInvalidLocations: !0,
					doNotLocateOnRestrictedElements: !0,
					outSpatialReference: this.get("map").spatialReference
				};
				this.get("routeParams") || (this.routeParams = {});
				var i = new rt;
				this.routeParams = s.mixin(i, t, this.get("routeParams"), e)
			},
			_markWPsForRemovalAfterUserChangedStopSequence: function (t, e) {
				for (var s = t - 1, i = []; s >= 0 && this._isStopAWaypoint(this.stops[s]); )
					i.push(this.stops[s]), s--;
				for (s = t + 1; s < this.stops.length && this._isStopAWaypoint(this.stops[s]); )
					i.push(this.stops[s]), s++;
				if (e > t)
					for (s = e + 1; s < this.stops.length && this._isStopAWaypoint(this.stops[s]); )
						i.push(this.stops[s]), s++;
				else if (void 0 !== e)
					for (s = e - 1; s >= 0 && this._isStopAWaypoint(this.stops[s]); )
						i.push(this.stops[s]), s--;
				else if (this.returnToStart && 0 === t)
					for (s = this.stops.length - 1; s >= 0 && this._isStopAWaypoint(this.stops[s]); )
						i.push(this.stops[s]), s--;
				return i
			},
			_removeSomeWaypoints: function (t) {
				for (var e = new P, s = [], i = 0; i < t.length; i++) {
					var o = r.indexOf(this.stops, t[i]);
					o > -1 && s.push(this._removeStop(o, !0, !0))
				}
				return A(s).always(e.resolve),
				e.promise
			},
			_modifyStopSequence: function (t, e) {
				var i,
				r = this._dnd.getAllNodes(),
				o = new P,
				n = [];
				if (r.length)
					if (this._removeLocateButtonVisibilityEvents(), arguments.length && void 0 !== t)
						if (t >= 0 && e >= 0 && t < this.stops.length && e < this.stops.length && t !== e) {
							var a = this._markWPsForRemovalAfterUserChangedStopSequence(t, e),
							h = this.stops.splice(t, 1);
							for (this.stops.splice(e, 0, h[0]), i = 0; i < this.stops.length; i++)
								n.push(this._updateStop(this.stops[i], i));
							n.push(this._removeSomeWaypoints(a)),
							A(n).always(s.hitch(this, function () {
									this._solveAndZoom().always(o.resolve)
								}))
						} else
							o.reject("Invalid From and To values.");
					else {
						for (i = 0; i < this.stops.length; )
							this._isStopAWaypoint(this.stops[i]) ? n.push(this._removeStop(i, !0, !0)) : i++;
						A(n).always(s.hitch(this, function () {
								for (n = [], this.stops.reverse(), i = 0; i < this.stops.length; i++)
									n.push(this._updateStop(this.stops[i], i));
								A(n).always(s.hitch(this, function () {
										this._solveAndZoom().always(o.resolve)
									}))
							}))
					}
				else
					o.resolve();
				return o.promise
			},
			_setMenuNodeValues: function () {
				"traffic" !== arguments[0] && this._clearDisplayBeforeSolve();
				var t = this.get("optimalRoute");
				if (this._findOptimalOrderNode && w.set(this._findOptimalOrderNode, "checked", t), this._returnToStartNode) {
					var e = this.returnToStart && !this.maxStopsReached && this.stops[0] && (!this.stops[0].feature || this._isStopLocated(this.stops[0]));
					w.set(this._returnToStartNode, "checked", e),
					this.set("returnToStart", e),
					this.maxStopsReached && this.returnToStart && this._showMessage(C.widgets.directions.error.maximumStops)
				}
				if (!this.returnToStart && !this._incrementalSolveStopRange && this.directions) {
					this._incrementalSolveStopRange = {
						start: this.stops.length - 1,
						end: this.stops.length
					},
					this._clearRouteGraphics(),
					this._incrementalSolveStopRange = null;
					for (var s = this.stops.length; s-- > 0 && this._isStopAWaypoint(this.stops[s]); )
						this._removeSomeWaypoints([this.stops[s]])
				}
				this._useTrafficNode && (w.set(this._useTrafficNode, "checked", this.traffic), this.traffic ? (this._updateMapTimeExtent(), this._addTrafficLayer()) : (this._removeTrafficLayer(), this._restoreMapTimeExtent()));
				var i = this.get("directionsLengthUnits");
				switch (i) {
				case O.KILOMETERS:
					w.set(this._useKilometersNode, "checked", !0),
					w.set(this._useMilesNode, "checked", !1);
					break;
				case O.MILES:
					w.set(this._useKilometersNode, "checked", !1),
					w.set(this._useMilesNode, "checked", !0)
				}
				S.set(this._printButton, "display", this.showPrintPage ? "inline-block" : "none"),
				S.set(this._saveMenuButton, "display", this.showSaveButton && this.owningSystemUrl ? "inline-block" : "none"),
				this._showMilesKilometersOption(),
				this._showClearButton()
			},
			_optionsMenu: function () {
				if (this._useTrafficItemNode && S.set(this._useTrafficItemNode, "display", this.get("showTrafficOption") ? "block" : "none"), this._returnToStartItemNode && S.set(this._returnToStartItemNode, "display", this.get("showReturnToStartOption") ? "block" : "none"), this._findOptimalOrderItemNode && S.set(this._findOptimalOrderItemNode, "display", this.get("showOptimalRouteOption") && this._getStopCount() > 3 ? "block" : "none"), this.stops.length >= this.get("minStops"))
					T.add(this._widgetContainer, this._css.stopsOptionsOptionsEnabledClass);
				else if (T.remove(this._widgetContainer, this._css.stopsOptionsOptionsEnabledClass), this._optionsMenuNode) {
					var t = S.get(this._optionsMenuNode, "display");
					"block" === t && this._toggleOptionsMenu()
				}
			},
			_stopsRemovable: function () {
				this._dnd.getAllNodes().length - this._getWaypointCount() > 2 ? T.add(this._widgetContainer, this._css.stopsRemovableClass) : T.remove(this._widgetContainer, this._css.stopsRemovableClass)
			},
			_checkMaxStops: function () {
				this.set("maxStopsReached", this._getStopCount() + this._getWaypointCount() + (this.returnToStart ? 1 : 0) >= this.maxStops),
				this._showAddDestination()
			},
			_updateThemeWatch: function () {
				var t = arguments[1],
				e = arguments[2];
				T.remove(this.domNode, t),
				T.add(this.domNode, e)
			},
			_toggleOptionsMenu: function () {
				var t = S.get(this._optionsMenuNode, "display");
				"block" === t ? (S.set(this._optionsMenuNode, "display", "none"), T.remove(this._optionsButtonNode, "esriStopsOptionsOpen"), this._optionsButtonNode.innerHTML = C.widgets.directions.showOptions) : (S.set(this._optionsMenuNode, "display", "block"), T.add(this._optionsButtonNode, "esriStopsOptionsOpen"), this._optionsButtonNode.innerHTML = C.widgets.directions.hideOptions)
			},
			_toggleSaveMenu: function (t) {
				var e = S.get(this._saveMenuNode, "display");
				"block" === e || t === !1 ? (S.set(this._saveMenuNode, "display", "none"), T.remove(this._saveMenuButton, this._css.stopsPressedButtonClass)) : (this.clearMessages(), S.set(this._saveMenuNode, "display", "block"), T.add(this._saveMenuButton, this._css.stopsPressedButtonClass), this._enableSharing().then(s.hitch(this, function () {
							if (this._outputLayer.setValue(this.routeLayer.title ? this.routeLayer.title : this.directions && this.directions.routeName), this.routeLayer.ownerFolder)
								for (var t = this._folderSelector.store.objectStore.data, e = 0; e < t.length; e++)
									if (t[e].folderId === this.routeLayer.ownerFolder) {
										this._folderSelector.getValue() !== t[e].id && (this._folderSelector._interractive = !1, this._folderSelector.setValue(t[e].id));
										break
									}
							this._enableButton(this._saveButton, this.routeLayer.isItemOwner || !this._userCanCreatePortalItem),
							this._outputLayer.set("disabled", !1)
						})))
			},
			_showToolbar: function () {
				T[this.stops.length < this.maxStops && this.canModifyStops || this.canModifyWaypoints ? "add" : "remove"].call(N, this._widgetContainer, this._css.addStopsClass)
			},
			_showAddDestination: function () {
				this._showToolbar(),
				this._addDestinationNode.style.display = this.stops.length < this.maxStops && this.canModifyStops ? "inline" : "none"
			},
			_showMapClickActiveButton: function () {
				this._showToolbar(),
				this._activateButtonNode.style.display = this.canModifyStops || this.canModifyWaypoints ? "inline-block" : "none"
			},
			_getAbsoluteUrl: function (e) {
				return e = t.toUrl(e),
				/^https?\:/i.test(e) ? e : /^\/\//i.test(e) ? mt + e : /^\//i.test(e) ? mt + "//" + window.location.host + e : e
			},
			_getManeuverImage: function (t) {
				if (t) {
					var e = "esri/dijit/images/Directions/maneuvers/",
					s = ".png";
					return "esriDMTStop" === t || "esriDMTDepart" === t ? "" : this._getAbsoluteUrl(e + t + s)
				}
				return ""
			},
			_loadPrintDirections: function (t) {
				var e = this.get("printTemplate"),
				i = new P;
				this.directionsLengthUnits !== this.routeParams.directionsLengthUnits ? this.getDirections().then(i.resolve, i.reject) : i.resolve(),
				i.then(s.hitch(this, function () {
						if (!e && this.directions) {
							var s,
							i = this._getAbsoluteUrl("esri/dijit/css/Directions.css"),
							r = this._getAbsoluteUrl("esri/dijit/css/DirectionsPrint.css"),
							o = this._getAbsoluteUrl("esri/dijit/images/Directions/print-logo.png");
							s = y.isBodyLtr() ? "ltr" : "rtl",
							e = "",
							e += "<!DOCTYPE HTML>",
							e += '<html lang="en" class="' + this.get("theme") + '" dir="' + s + '">',
							e += "<head>",
							e += '<meta charset="utf-8">',
							e += '<meta http-equiv="X-UA-Compatible" content="IE=Edge,chrome=1">',
							e += "<title>" + this.get("directions").routeName + "</title>",
							e += '<link rel="stylesheet" media="screen" type="text/css" href="' + i + '" />',
							e += '<link rel="stylesheet" media="print" type="text/css" href="' + r + '" />',
							e += "</head>",
							e += '<body class="' + this._css.esriPrintPageClass + '">',
							e += '<div class="' + this._css.esriPrintBarClass + '">',
							e += '<div class="' + this._css.esriCloseButtonClass + '" title="' + C.common.close + '" onclick="window.close();">' + C.common.close + "</div>",
							e += '<div id="printButton" class="' + this._css.esriPrintButtonClass + '" title="' + C.widgets.directions.print + '" onclick="window.print();">' + C.widgets.directions.print + "</div>",
							e += "</div>",
							e += '<div class="' + this._css.esriPrintMainClass + '">',
							e += '<div class="' + this._css.esriPrintHeaderClass + '">',
							e += '<img class="' + this._css.esriPrintLogoClass + '" src="' + o + '" />',
							e += '<div class="' + this._css.esriPrintNameClass + '">' + this.get("directions").routeName + "</div>",
							e += this._renderDirectionsSummary(this.directions),
							t && (e += '<div id="divMap" class="esriPrintMap esriPrintWait"></div>', e += '<hr class="esriNoPrint"/>'),
							e += '<div id="print_helper"></div>',
							e += '<textarea onkeyup="document.getElementById(\'print_helper\').innerHTML=this.value;" id="print_area" class="' + this._css.esriPrintNotesClass + '" placeholder="' + C.widgets.directions.printNotes + '"></textarea>',
							e += '<div class="' + this._css.clearClass + '"></div>',
							e += "</div>",
							e += '<div class="' + this._css.esriPrintDirectionsClass + '">',
							e += this._renderDirectionsTable(this.directions),
							e += "</div>",
							e += '<div class="' + this._css.esriPrintFooterClass + '">',
							e += "<p>" + C.widgets.directions.printDisclaimer + "</p>",
							e += "</div>",
							e += "</div>",
							e += "</body>",
							e += "</html>"
						}
						this._printWindow.document.open("text/html", "replace"),
						this._printWindow.document.write(e),
						this._printWindow.document.close()
					}))
			},
			_printDirections: function () {
				if (this.directions) {
					var e = screen.width / 2,
					i = screen.height / 1.5,
					r = screen.width / 2 - e / 2,
					o = screen.height / 2 - i / 2,
					n = "toolbar=no, location=no, directories=no, status=yes, menubar=no, scrollbars=yes, resizable=yes, width=" + e + ", height=" + i + ", top=" + o + ", left=" + r;
					this.get("printPage") ? (window.directions = this.get("directions"), window.open(this.get("printPage"), "directions_widget_print", n, !0)) : (this._printWindow = window.open("", "directions_widget_print", n, !0), this._loadPrintDirections(!!this._printService), this._printService && t(["dojo/_base/window"], s.hitch(this, function (t) {
								this.zoomToFullRoute().then(s.hitch(this, function () {
										this._printService.execute(this._printParams, s.hitch(this, function (e) {
												t.withDoc(this._printWindow.document, function () {
													var t = v.byId("divMap");
													t && (T.remove(t, "esriPrintWait"), T.add(t, "esriPageBreak"), N.create("img", {
															src: e.url,
															"class": "esriPrintMapImg"
														}, t))
												})
											}), s.hitch(this, function (e) {
												t.withDoc(this._printWindow.document, function () {
													var t = v.byId("divMap");
													t && T.remove(t, "esriPrintWait")
												}),
												console.error("Error while calling print service:\n " + e)
											}))
									}))
							})))
				}
			},
			_enableButton: function (t, e) {
				var s = e || void 0 === e;
				T[s ? "remove" : "add"].apply(this, [t, "esriDisabledDirectionsButton"]),
				t._enabled = s
			},
			_clearStopsStatusAttr: function () {
				for (var t = 0; t < this.stops.length; t++)
					this.stops[t].feature && this.stops[t].feature.attributes && (this.stops[t].feature.attributes.Status = void 0)
			},
			_enableSharing: function () {
				var e = new P;
				return !this._naRouteSharing && this.owningSystemUrl ? t(["esri/tasks/NARouteSharing"], s.hitch(this, function (t) {
						this._naRouteSharing = new t(this.owningSystemUrl, this.map.spatialReference),
						this._folderSelector || (this._folderSelector = new h({
									className: "esriFoldersDDL",
									style: "width: 100%;",
									sortByLabel: !1,
									disabled: !0,
									_interractive: !0,
									onChange: s.hitch(this, function () {
										this._folderSelector._interractive ? this._enableButton(this._saveButton, !this.routeLayer.itemId) : this._folderSelector._interractive = !0
									})
								}, this._folderSelectorContainer), this._folderSelector.startup(), this._outputLayer = new c({
									style: "width: 100%",
									required: !0,
									trim: !0,
									regExp: '[^&|<|>|%|#|?|\\|"|/|+]+',
									maxLength: 98,
									disabled: !0,
									onKeyPress: s.hitch(this, function () {
										this._enableButton(this._saveButton, !this.routeLayer.itemId || !this._userCanCreatePortalItem)
									}),
									onFocus: s.hitch(this, function () {
										this.map && this.map.disableKeyboardNavigation()
									}),
									onBlur: s.hitch(this, function () {
										this.map && this.map.enableKeyboardNavigation()
									})
								}, this._outputLayerContainer), this._outputLayer.startup()),
						this._naRouteSharing.getFolders().then(s.hitch(this, function (t) {
								for (var i = [], r = 0; r < t.length; r++)
									i.push({
										id: t[r].url,
										folderId: t[r].id,
										label: t[r].title
									});
								this._folderSelector.setStore(new p({
										objectStore: new d({
											data: i
										})
									})),
								this._naRouteSharing.canCreateItem().then(s.hitch(this, function (t) {
										this._folderSelector.set("disabled", !t),
										this._userCanCreatePortalItem = t
									})),
								this._enableButton(this._saveButton),
								e.resolve()
							}), s.hitch(this, function (t) {
								console.log(t),
								this._naRouteSharing = null,
								this._toggleSaveMenu(!1),
								e.reject(t)
							}))
					})) : this.owningSystemUrl ? e.resolve() : (e.reject(new Error("Owning system is not defined, or the Directions widget is not done initializing.")), this._naRouteSharing = null),
				e.promise
			},
			_storeRouteUI: function () {
				var t = new P;
				return this._outputLayer && this._outputLayer.isValid() ? this._storeRoute(this._outputLayer.getValue(), this._folderSelector.getValue(), this._folderSelector.store.objectStore.get(this._folderSelector.getValue()).folderId).then(t.resolve, t.reject) : (this._outputLayer && this._outputLayer.focus(), t.reject(new Error("Need result layer name specified."))),
				t.promise
			},
			_storeRoute: function (t, e, i) {
				var o = new P,
				n = this.directions;
				if (this._savingRoute || !this._naRouteSharing)
					return this._saveButton.blur(), o.reject(new Error("Not ready to store route.")), o.promise;
				if (this.clearMessages(), o.then(null, s.hitch(this, function (t) {
							console.log("ERR", t),
							this._showMessage(C.widgets.directions.error.cantSaveRoute)
						})), o.promise.always(s.hitch(this, function () {
							this._enableButton(this._saveButton),
							S.set(this._saveAsButton, "display", this.routeLayer.itemId && this._userCanCreatePortalItem ? "inline-block" : "none"),
							this._showLoadingSpinner(),
							this._savingRoute = !1
						})), n && n.features && n.features.length && this.stops && this.stops.length)
					if (this.owningSystemUrl)
						if (this.routeParams && this.routeParams.travelMode)
							if (t && e) {
								var a,
								h,
								c,
								u = s.hitch(this, function (t) {
										return this._naRouteSharing.getAttributeUnits(t, this.serviceDescription)
									}),
								l = this.routeParams.travelMode.timeAttributeName,
								d = this.routeParams.travelMode.distanceAttributeName,
								p = u(l),
								m = u(d),
								_ = this.routeParams.directionsLengthUnits || this.serviceDescription.directionsLengthUnits,
								g = this._naRouteSharing.toMeters,
								f = this._naRouteSharing.toMinutes,
								v = function (t, e, s) {
									var i;
									for (var o in t)
										t.hasOwnProperty(o) && 0 === o.indexOf(e) && -1 === r.indexOf(s, o.substr(e.length)) && (i = i || {}, i[o.substr(e.length)] = t[o]);
									return i
								},
								y = [],
								T = [],
								w = [];
								if (l && p && d && m) {
									var b = [],
									M = this.routeParams.accumulateAttributes || this.serviceDescription.accumulateAttributeNames || [],
									D = this.serviceDescription.networkDataset.networkAttributes;
									for (a = 0; a < D.length; a++)
										"esriNAUTCost" === D[a].usageType && -1 === r.indexOf(M, D[a].name) && D[a].name !== l && D[a].name !== d && b.push(D[a].name);
									var L = this.get("stops"),
									N = {
										xmin: 1 / 0,
										ymin: 1 / 0,
										xmax:  - (1 / 0),
										ymax:  - (1 / 0)
									};
									for (a = 0; a < L.length; a++)
										if (L[a].feature && L[a].feature.toJson) {
											var A = L[a].feature.toJson(),
											x = A.attributes,
											R = A.geometry,
											B = this._naRouteSharing.getUTCOffset(x.ArriveTime, x.ArriveTimeUTC),
											E = this._naRouteSharing.getUTCOffset(x.DepartTime, x.DepartTimeUTC);
											s.mixin(N, {
												xmin: N.xmin > R.x ? R.x : N.xmin,
												ymin: N.ymin > R.y ? R.y : N.ymin,
												xmax: N.xmax < R.x ? R.x : N.xmax,
												ymax: N.ymax < R.y ? R.y : N.ymax
											}),
											A.attributes = {
												__OBJECTID: a + 1,
												CurbApproach: x.CurbApproach,
												ArrivalCurbApproach: x.ArriveCurbApproach,
												DepartureCurbApproach: x.DepartCurbApproach,
												Name: x.Name === this._waypointName ? C.widgets.directions.waypoint : x.Name === this._userDefinedStopName ? L[a].name : x.Name,
												RouteName: n.routeName,
												Sequence: x.Sequence,
												Status: x.Status,
												LocationType: x.isWaypoint ? 1 : 0,
												TimeWindowStart: x.TimeWindowStart,
												TimeWindowEnd: x.TimeWindowEnd,
												TimeWindowStartUTCOffset: B,
												TimeWindowEndUTCOffset: B,
												ServiceMinutes: f(x["Attr_" + l], p),
												ServiceMeters: g(x["Attr_" + d], m),
												ServiceCosts: I.stringify(v(x, "Attr_", b)),
												CumulativeMinutes: f(x["Cumul_" + l], p),
												CumulativeMeters: g(x["Cumul_" + d], m),
												CumulativeCosts: I.stringify(v(x, "Cumul_", b)),
												LateMinutes: f(x["Violation_" + l], p),
												WaitMinutes: f(x["Wait_" + l], p),
												ArrivalTime: this._naRouteSharing.toUTCTime(x.ArriveTime, B),
												DepartureTime: this._naRouteSharing.toUTCTime(x.DepartTime, E),
												ArrivalUTCOffset: B,
												DepartureUTCOffset: E
											},
											y.push(A)
										}
									var O = 0,
									k = function (t) {
										try {
											t.strings = I.parse(t.strings)
										} catch (e) {
											V.strings = void 0
										}
										if (t.strings && t.strings.length)
											for (var s = 0; s < t.strings.length; s++)
												if ("esriDSTGeneral" === t.strings[s].stringType)
													return t.strings[s].string
									},
									W = function (t, e) {
										for (var s = [], i = 0; i < (t || []).length; i++)
											t[i].stringType !== e && e || s.push(t[i].string);
										return s.length ? s.toString() : void 0
									},
									U = function (t) {
										switch (t) {
										case "esriDMTStop":
											return "esriDPTManeuverArrive";
										case "esriDMTDepart":
											return "esriDPTManeuverDepart";
										case "esriDMTDoorPassage":
											return "esriDPTManeuverDoor";
										case "esriDMTBearLeft":
											return "esriDPTManeuverBearLeft";
										case "esriDMTBearRight":
											return "esriDPTManeuverBearRight";
										case "esriDMTElevator":
											return "esriDPTManeuverElevator";
										case "esriDMTEscalator":
											return "esriDPTManeuverEscalator";
										case "esriDMTFerry":
											return "esriDPTManeuverFerryOn";
										case "esriDMTEndOfFerry":
											return "esriDPTManeuverFerryOff";
										case "esriDMTForkCenter":
											return "esriDPTManeuverForkCenter";
										case "esriDMTForkLeft":
											return "esriDPTManeuverForkLeft";
										case "esriDMTForkRight":
											return "esriDPTManeuverForkRight";
										case "esriDMTPedestrianRamp":
											return "esriDPTManeuverPedestrianRamp";
										case "esriDMTRampLeft":
											return "esriDPTManeuverRampLeft";
										case "esriDMTRampRight":
											return "esriDPTManeuverRampRight";
										case "esriDMTRoundabout":
											return "esriDPTManeuverRoundabout";
										case "esriDMTTurnLeft":
											return "esriDPTManeuverTurnLeft";
										case "esriDMTLeftLeft":
											return "esriDPTManeuverTurnLeftLeft";
										case "esriDMTLeftRight":
											return "esriDPTManeuverTurnLeftRight";
										case "esriDMTTurnRight":
											return "esriDPTManeuverTurnRight";
										case "esriDMTRightLeft":
											return "esriDPTManeuverTurnRightLeft";
										case "esriDMTRightRight":
											return "esriDPTManeuverTurnRightRight";
										case "esriDMTSharpLeft":
											return "esriDPTManeuverSharpLeft";
										case "esriDMTSharpRight":
											return "esriDPTManeuverSharpRight";
										case "esriDMTStraight":
											return "esriDPTManeuverStraight";
										case "esriDMTStrairs":
											return "esriDPTManeuverStairs";
										case "esriDMTUTurn":
											return "esriDPTManeuverUTurn"
										}
										return "esriDPTUnknown"
									};
									for (a = 0; a < n.featuresWithWaypoints.length; a++) {
										var G = n.featuresWithWaypoints[a],
										j = G.toJson(),
										F = j.attributes,
										H = j.geometry && j.geometry.paths && j.geometry.paths[0] || [],
										q = j.geometry.hasM || !1 || H[0] && 3 === H[0].length;
										c = this._naRouteSharing.getUTCOffset(F.ETA, F.arriveTimeUTC),
										j.attributes = {
											__OBJECTID: a + 1,
											Sequence: ++O,
											StopID: function () {
												var t = void 0,
												e = G._associatedStopWithReturnToStart && G._associatedStopWithReturnToStart.attributes.Sequence;
												if (e)
													for (var s = 0; s < y.length; s++)
														if (y[s].attributes.Sequence === e) {
															t = y[s].attributes.__OBJECTID;
															break
														}
												return t
											}
											(),
											DirectionPointType: U(F.maneuverType),
											DisplayText: G._associatedStopWithReturnToStart && G._associatedStopWithReturnToStart.attributes.isWaypoint ? F.text.replace(this._waypointName, C.widgets.directions.waypoint) : F.text,
											ArrivalTime: this._naRouteSharing.toUTCTime(F.ETA, c),
											ArrivalUTCOffset: c,
											Azimuth: void 0,
											Name: W(n.stringsWithWaypoints[a], "esriDSTStreetName"),
											AlternateName: W(n.stringsWithWaypoints[a], "esriDSTAltName"),
											ExitName: W(n.stringsWithWaypoints[a], "esriDSTExit"),
											IntersectingName: W(n.stringsWithWaypoints[a], "esriDSTCrossStreet"),
											BranchName: W(n.stringsWithWaypoints[a], "esriDSTBranch"),
											TowardName: W(n.stringsWithWaypoints[a], "esriDSTToward"),
											ShortVoiceInstruction: void 0,
											VoiceInstruction: void 0,
											Level: void 0
										},
										delete j.symbol,
										delete j.infoTemplate,
										H.length ? (j.geometry = {
												x: H[0][0],
												y: H[0][1],
												spatialReference: j.geometry.spatialReference
											}, q && s.mixin(j.geometry, {
												m: H[0][2]
											})) : delete j.geometry,
										T.push(j),
										j = G.toJson(),
										F = j.attributes,
										H = j.geometry && j.geometry.paths && j.geometry.paths[0] || [];
										var Z = !0;
										for (h = 0; h < H.length - 1; h++)
											if (H[h][0] !== H[h + 1][0] || H[h][1] !== H[h + 1][1]) {
												Z = !1;
												break
											}
										Z || (j.attributes = {
												DirectionPointID: a + 1,
												DirectionLineType: "esriDLTSegment",
												Meters: g(F.length, _),
												Minutes: f(F.time, p),
												FromLevel: void 0,
												ToLevel: void 0
											}, j.geometry.hasM = q, delete j.symbol, delete j.infoTemplate, w.push(j));
										var V = n.eventsWithWaypoints[j.attributes.Sequence] || [];
										for (h = 0; h < V.length; h++) {
											var z = V[h].toJson(),
											K = z.attributes;
											c = this._naRouteSharing.getUTCOffset(K.ETA, K.arriveTimeUTC),
											z.attributes = {
												Sequence: ++O,
												DirectionPointType: "esriDPTEvent",
												DisplayText: k(K),
												ArrivalTime: this._naRouteSharing.toUTCTime(K.ETA, c),
												ArrivalUTCOffset: c,
												Name: K.strings
											},
											T.push(z)
										}
									}
									var J,
									Q,
									Y = this.routeParams.barriers && this.routeParams.barriers.features || [],
									$ = [];
									for (a = 0; a < Y.length; a++)
										J = Y[a].toJson(), Q = J.attributes, J.attributes = {
											BarrierType: Q.BarrierType || 0,
											FullEdge: Q.FullEdge || !1,
											AddedCost: Q["Attr_" + this._getImpedanceAttribute().name] || 0,
											Costs: I.stringify(v(Q, "Attr_", b)),
											CurbApproach: Q.CurbApproach || 0,
											Name: Q.Name
										},
									$.push(J);
									Y = this.routeParams.polylineBarriers && this.routeParams.polylineBarriers.features || [];
									var X = [];
									for (a = 0; a < Y.length; a++)
										J = Y[a].toJson(), Q = J.attributes, J.attributes = {
											BarrierType: Q.BarrierType || 0,
											ScaleFactor: Q["Attr_" + this._getImpedanceAttribute().name] || 1,
											Costs: I.stringify(v(Q, "Attr_", b)),
											Name: Q.Name
										},
									X.push(J);
									Y = this.routeParams.polygonBarriers && this.routeParams.polygonBarriers.features || [];
									var tt = [];
									for (a = 0; a < Y.length; a++)
										J = Y[a].toJson(), Q = J.attributes, J.attributes = {
											BarrierType: Q.BarrierType || 0,
											ScaleFactor: Q["Attr_" + this._getImpedanceAttribute().name] || 1,
											Costs: I.stringify(v(Q, "Attr_", b)),
											Name: Q.Name
										},
									tt.push(J);
									var et = {
										geometry: n.mergedGeometry,
										attributes: {
											RouteName: n.routeName,
											TotalMinutes: f(n.totalTime, p),
											TotalMeters: g(n.totalLength, _),
											TotalLateMinutes: function () {
												for (var t = 0, e = 0; e < y.length; e++)
													t += y[e].attributes.LateMinutes || 0;
												return t
											}
											(),
											TotalWaitMinutes: function () {
												for (var t = 0, e = 0; e < y.length; e++)
													t += y[e].attributes.WaitMinutes || 0;
												return t
											}
											(),
											TotalCosts: y[y.length - 1].attributes.CumulativeCosts,
											StartTime: "none" !== this.startTime ? y[0].attributes.ArrivalTime : null,
											EndTime: "none" !== this.startTime ? y[y.length - 1].attributes.DepartureTime : null,
											StartUTCOffset: y[0].attributes.ArrivalUTCOffset,
											EndUTCOffset: y[y.length - 1].attributes.DepartureUTCOffset,
											Messages: I.stringify(this._solverMessages),
											AnalysisSettings: I.stringify({
												travelMode: s.hitch(this, function () {
													var t = s.clone(this.routeParams.travelMode);
													return "&lt;" === t.name.substr(0, 4) && "&gt;" === t.name.substr(t.name.length - 4, 4) && (t.name = t.name.substr(4, t.name.length - 8)),
													t
												})(),
												directionsLanguage: this.routeParams.directionsLanguage || this.serviceDescription.directionsLanguage,
												startTimeIsUTC: this.routeParams.startTimeIsUTC,
												timeWindowsAreUTC: this.routeParams.timeWindowsAreUTC,
												findBestSequence: this.routeParams.findBestSequence,
												preserveFirstStop: this.routeParams.preserveFirstStop,
												preserveLastStop: this.routeParams.preserveLastStop,
												accumulateAttributeNames: this.routeParams.accumulateAttributes || this.serviceDescription.accumulateAttributeNames
											})
										}
									};
									this._enableButton(this._saveButton, !1),
									this._savingRoute = !0,
									this._showLoadingSpinner(!0),
									s.hitch(this, function () {
										var t = new P;
										return this._printService ? this.zoomToFullRoute().then(s.hitch(this, function () {
												var e = this._printParams.template,
												s = e.exportOptions;
												e.exportOptions = {
													width: 200,
													height: 133,
													dpi: 96
												},
												this._printService.execute(this._printParams, function (i) {
													e.exportOptions = s,
													t.resolve(i.url)
												}, function (i) {
													e.exportOptions = s,
													console.error("Error while calling print service:\n " + i),
													t.resolve()
												})
											})) : t.resolve(),
										t.promise
									})().then(s.hitch(this, function (r) {
											var a = {
												folder: e,
												name: t,
												stops: y,
												directionPoints: T,
												directionLines: w,
												barriers: $,
												polylineBarriers: X,
												polygonBarriers: tt,
												extent: function () {
													return s.mixin(s.clone(n.extent), {
														xmin: N.xmin > n.extent.xmin ? n.extent.xmin : N.xmin,
														ymin: N.ymin > n.extent.ymin ? n.extent.ymin : N.ymin,
														xmax: N.xmax < n.extent.xmax ? n.extent.xmax : N.xmax,
														ymax: N.ymax < n.extent.ymax ? n.extent.ymax : N.ymax
													})
												}
												(),
												routeInfo: et,
												thumbnail: r
											};
											this._userCanCreatePortalItem ? this._naRouteSharing.store(a, this.routeLayer.itemId, "1.0.0").then(s.hitch(this, function (t) {
													if (t.success) {
														var e = this._naRouteSharing.portal,
														r = "//" + (e.isPortal ? e.portalHostname : e.urlKey + "." + e.customBaseUrl);
														this._toggleSaveMenu(),
														this._showMessage(C.widgets.directions.routeIsSaved + "<br/><a class='esriLinkButton' target='_blank' href='" + r + "/home/item.html?id=" + t.id + "'>" + C.widgets.directions.share + "</a>", !0),
														this.routeLayer.itemId ? this.onRouteItemUpdated(t.id) : this.onRouteItemCreated(t.id),
														s.mixin(this.routeLayer, {
															itemId: t.id,
															title: a.name,
															isItemOwner: !0,
															ownerFolder: i
														}),
														o.resolve(t)
													} else
														o.reject(t)
												}), o.reject) : this._naRouteSharing.createFeatureCollection(a, "1.0.0").then(s.hitch(this, function (t) {
													o.resolve(t),
													this._toggleSaveMenu(),
													this.onFeatureCollectionCreated(t)
												}), o.reject)
										}))
								} else
									o.reject(new Error("Cannot deduce the impedance used."))
							} else
								o.reject(new Error("Missing required parameter: layerName, folder must be specified."));
						else
							o.reject(new Error("Shared route must be built using a Travel Mode."));
					else
						o.reject(new Error("Cannot store route: owning system to store routes is not defined. Please specify Portal or ArcGIS Online Url in constructor."));
				else
					o.reject(new Error("No route to share. Build a route first."));
				return o.promise
			},
			_loadRoute: function (t) {
				var e = new P;
				return e.promise.always(s.hitch(this, function () {
						this._showLoadingSpinner(!1),
						S.set(this._saveAsButton, "display", this.routeLayer.itemId && this._userCanCreatePortalItem ? "inline-block" : "none"),
						this._enableButton(this._saveButton, this.routeLayer.isItemOwner)
					})),
				this._reset().then(s.hitch(this, function () {
						this._showLoadingSpinner(!0),
						this._enableSharing().then(s.hitch(this, function () {
								var i,
								r,
								o,
								n,
								a = s.clone(this.serviceDescription),
								h = s.hitch(this, function (e) {
										s.mixin(this.routeLayer, {
											itemId: t,
											title: e.title,
											isItemOwner: e.isItemOwner,
											ownerFolder: e.ownerFolder
										})
									});
								a.directionsLengthUnits = this.directionsLengthUnits,
								this._naRouteSharing.load(t, a).then(s.hitch(this, function (t) {
										if (t.routeParameters) {
											s.mixin(this.routeParams, t.routeParameters),
											this.routeParams.accumulateAttributes = t.routeParameters.accumulateAttributeNames,
											this.set("optimalRoute", this.routeParams.findBestSequence),
											this.startTime = this.routeParams.startTime ? this.routeParams.startTime : "none",
											this._setStartTime(void 0, void 0, this.startTime);
											var c = t.routeParameters.travelMode,
											u = c ? this._getCostAttribute(c.impedanceAttributeName) : void 0,
											l = u ? this._isTimeUnits(u.units) ? "Time" : "Distance" : "";
											if (n = a.supportedTravelModes && a.supportedTravelModes.length ? this._travelModeSelector.store.objectStore.data.slice() : [], canUseRouteLayerTM = 1 !== t.successCode && l, canUseRouteLayerTM) {
												var m = "AUTOMOBILE" === c.type ? "Driving" : "TRUCK" === c.type ? "Trucking" : "WALK" === c.type ? "Walking" : "Other";
												c.name = "&lt;" + c.name + "&gt;",
												this.serviceDescription.supportedTravelModes = (a.supportedTravelModes || []).concat(c),
												n.push({
													id: c.name,
													label: '<div class="esriTravelModesDirectionsIcon esriTravelModesType' + m + l + '">&nbsp;</div><div class="esriTravelModesTypeName">' + c.name + "</div>"
												}),
												this._travelModeSelector.setStore(new p({
														objectStore: new d({
															data: n
														})
													})),
												this._travelModeSelector._interractive = !1,
												this._travelModeSelector.setValue(c.name)
											} else {
												var _ = !1;
												if (c && c.name)
													for (n = a.supportedTravelModes || [], i = 0; i < n.length; i++)
														if (n[i].name === c.name) {
															_ = !0,
															this._travelModeSelector._interractive = !1,
															this._travelModeSelector.setValue(c.name),
															this.routeParams.travelMode = n[i].impedanceAttributeName ? n[i] : n[i].itemId,
															t.loadMessages.push({
																message: C.widgets.directions.error.tmFromPortalSameName + " " + c.name,
																messageType: "Warning"
															});
															break
														}
												if (!_) {
													var g = a.defaultTravelMode;
													if (g && n) {
														for (i = 0; i < n.length; i++)
															if (n[i].id === g) {
																this.routeParams.travelMode = n[i].impedanceAttributeName ? n[i] : n[i].itemId,
																t.loadMessages.push({
																	message: C.widgets.directions.error.tmFromPortalDefault + " " + (n[i].name ? n[i].name : n[i].itemId),
																	messageType: "Warning"
																});
																break
															}
													} else
														this.routeParams.travelMode = null
												}
											}
											this._checkStartTimeUIAvailability()
										}
										if (t.solveResult && t.solveResult.routeResults) {
											var f,
											v,
											y,
											S = 1 / 0,
											T =  - (1 / 0),
											w = t.solveResult.routeResults[0].stops,
											b = t.solveResult.routeResults[0].directions.features,
											M = {};
											for (i = 0; i < w.length; i++) {
												var D = w[i].attributes;
												if (D.isWaypoint)
													for (r = 0; r < b.length; r++)
														f = b[r].attributes, f._stopSequence === D.Sequence && (f.text = f.text.replace(D.Name, this._waypointName));
												o = D.Name + "_" + this._stopSequence++,
												M[o] = D.isWaypoint ? this._waypointName : D.Name,
												D.Name = o,
												D.Sequence < S && (null !== D.ArriveCurbApproach || null !== D.DepartCurbApproach) && (v = D.Name, S = D.Sequence),
												D.Sequence > T && (null !== D.ArriveCurbApproach || null !== D.DepartCurbApproach) && (y = D.Name, T = D.Sequence)
											}
											for (o = v + " - " + y, t.solveResult.routeResults[0].routeName = o, t.solveResult.routeResults[0].directions.routeName = o, i = 0; i < b.length; i++)
												if (f = b[i].attributes, void 0 !== f._stopSequence)
													for (r = 0; r < w.length; r++)
														if (f._stopSequence === w[r].attributes.Sequence) {
															o = w[r].attributes.Name,
															f.text = (f.text || "").replace(M[o], o),
															delete f._stopSequence;
															break
														}
											var L = w[w.length - 1];
											w[0].geometry.x === L.geometry.x && w[0].geometry.y === L.geometry.y && (this._returnToStartStop = this._addStopWrapperToGraphic(new E(L.geometry, null, L.attributes), M[L.attributes.Name]), this.set("returnToStart", !0)),
											this._solveResultProcessing(t.solveResult, M).then(s.hitch(this, function () {
													this._setStartTime(void 0, void 0, this.startTime),
													h(t),
													this.zoomToFullRoute(),
													e.resolve(t)
												}), e.reject)
										} else {
											if (t.routeParameters) {
												var N = t.routeParameters.stops.features;
												for (this.stops = [], i = 0; i < N.length; i++)
													this.stops.push(this._addStopWrapperToGraphic(N[i], N[i].attributes.Name)), this._updateStop(this.stops[i], i);
												this._setStops(),
												t.loadMessages.push({
													message: C.widgets.directions.routeLayerStopsOnly,
													messageType: "Warning"
												})
											} else
												t.loadMessages.push({
													message: C.widgets.directions.routeLayerEmpty,
													messageType: "Warning"
												});
											h(t),
											e.resolve(t)
										}
										for (i = 0; i < t.loadMessages.length; i++)
											this._showMessage(t.loadMessages[i].message)
									}), s.hitch(this, function (t) {
										e.reject(t),
										this._showMessage("GWM_0003" === t.messageCode ? C.widgets.directions.error.accessDenied + this._naRouteSharing.portal.getPortalUser().username : C.widgets.directions.error.loadError)
									}))
							}), e.reject)
					}), e.reject),
				e.promise
			}
		});
	return _("extend-esri") && s.setObject("dijit.Directions", _t, R),
	_t
});
