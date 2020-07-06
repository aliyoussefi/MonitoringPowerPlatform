import {IInputs, IOutputs} from "./generated/ManifestTypes";
import { ApplicationInsights, IEventTelemetry } from '@microsoft/applicationinsights-web'
import { env } from "process";
import { Context } from "vm";

export class ApplicationInsightsControl implements ComponentFramework.StandardControl<IInputs, IOutputs> {
	pcfControlAppInsights = new ApplicationInsights({ config: {
		instrumentationKey: '<your instrumentation key>',
		enableResponseHeaderTracking: true,
		enableRequestHeaderTracking: true,
		enableAutoRouteTracking: true
		/* ...Other Configuration Options... */
	  } });
	  
	// Value of the field is stored and used inside the control 
	private _value:string;

	// PCF framework delegate which will be assigned to this object which would be called whenever any update happens. 
	private _notifyOutputChanged: () => void;	
	private questionText: HTMLInputElement;
	private answerText: HTMLLabelElement;
	private button: HTMLButtonElement;
	private span: HTMLSpanElement;

	// Reference to the control container HTMLDivElement
	// This element contains all elements of our custom control example
	private _container: HTMLDivElement;
	/**
	 * Empty constructor.
	 */
	constructor()
	{
		//Conflict with D365: https://github.com/microsoft/ApplicationInsights-JS/issues/953
		//https://github.com/microsoft/TypeScript/issues/31609
		if (!this.pcfControlAppInsights.core.isInitialized!()){
			//Use this to load the app insights dependencies for use.
			this.pcfControlAppInsights.loadAppInsights();
		}
	}

	/**
	 * Used to initialize the control instance. Controls can kick off remote server calls and other initialization actions here.
	 * Data-set values are not initialized here, use updateView.
	 * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to property names defined in the manifest, as well as utility functions.
	 * @param notifyOutputChanged A callback method to alert the framework that the control has new outputs ready to be retrieved asynchronously.
	 * @param state A piece of data that persists in one session for a single user. Can be set at any point in a controls life cycle by calling 'setControlState' in the Mode interface.
	 * @param container If a control is marked control-type='starndard', it will receive an empty div element within which it can render its content.
	 */
	public init(context: ComponentFramework.Context<IInputs>, notifyOutputChanged: () => void, state: ComponentFramework.Dictionary, container:HTMLDivElement)
	{
		//@ts-ignore
		//https://stackoverflow.com/questions/56506942/azure-appinsights-default-telemetryinitializer-javascript-sdk
		//Use this to set context properties

		var orgSettings = context.orgSettings;
		
		var response = context.webAPI.retrieveMultipleRecords("organization", "?$select=organizationid")
		.then((resp) => resp.entities[0].organizationid)
		.catch(console.error)

		var x = context.userSettings;
		var entityMetadata = context.utils.getEntityMetadata;

		var telemetryInitializer = (envelope) => {
			envelope.tags["ai.cloud.role"] = orgSettings._customControlExposedOrgSettings.organizationId;
			envelope.tags["ai.cloud.roleInstance"] = response;
			//envelope.tags["ai.session.id"] = window.URL;
			envelope.tags["ai.operation.name"] = ApplicationInsightsControl.name;
			envelope.baseData.properties = envelope.baseData.properties || {};
			envelope.baseData.properties["email"] = "test1@email.com";
			envelope.data.cloudRole = 'just checking in';
			}
		//Add the context properties
		this.pcfControlAppInsights.addTelemetryInitializer(telemetryInitializer);
		//Set user authenticated
		this.pcfControlAppInsights.setAuthenticatedUserContext(context.userSettings.userId, undefined, true);
		//Send Page View Event
		this.pcfControlAppInsights.trackPageView();
		this.pcfControlAppInsights.trackEvent({name: 'init PCF Control Called.'}, {
			"userId":context.userSettings.userId,
			"userName":context.userSettings.userName
		});

		this.span = document.createElement("span");	
	
		this.button = document.createElement("button");
		this.button.innerHTML = "Send to Application Insights";
		this.button.addEventListener("click", this.onButtonClick.bind(this));

		this._container = document.createElement("div");
		this._container.appendChild(this.span);
		this._container.appendChild(this.button);	
		container.appendChild(this._container);

		this._notifyOutputChanged = notifyOutputChanged;
	}	

	/**
	 * Button Event handler for the button created as part of this control
	 * @param event
	 */
		private onButtonClick(event: Event): void 
		{
			this.pcfControlAppInsights.trackEvent({name: 'onButtonClicked'});
			this.pcfControlAppInsights.trackPageView({name: 'some page'});
			this.pcfControlAppInsights.trackException({exception: new Error('some error')});
			this.pcfControlAppInsights.trackTrace({message: 'some trace'});
			this.pcfControlAppInsights.trackMetric({name: 'some metric', average: 42});
			this.pcfControlAppInsights.startTrackEvent("call complete");

		}

	/**
	 * Called when any value in the property bag has changed. This includes field values, data-sets, global values such as container height and width, offline status, control metadata values such as label, visible, etc.
	 * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to names defined in the manifest, as well as utility functions
	 */  
	public updateView(context: ComponentFramework.Context<IInputs>): void
	{
		// Add code to update control view
		this.pcfControlAppInsights.trackEvent({name: 'UpdateView PCF Control Called.'});
	}

	/** 
	 * It is called by the framework prior to a control receiving new data. 
	 * @returns an object based on nomenclature defined in manifest, expecting object[s] for property marked as “bound” or “output”
	 */
	public getOutputs(): IOutputs
	{		
		this.pcfControlAppInsights.trackEvent({name: 'getOutputs PCF Control Called.'});
		return {};
	}

	/** 
	 * Called when the control is to be removed from the DOM tree. Controls should use this call for cleanup.
	 * i.e. cancelling any pending remote calls, removing listeners, etc.
	 */
	public destroy(): void
	{
		this.pcfControlAppInsights.trackEvent({name: 'Destroy PCF Control Called.'});
		// Add code to cleanup control if necessary
	}
}