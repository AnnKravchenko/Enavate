import {IInputs, IOutputs} from "./generated/ManifestTypes";

interface IToggle {
	defaultValue: boolean | undefined,
	currentValue: boolean | undefined,
	trueLable: string,
	falseLabel: string
}

export class TestControl implements ComponentFramework.StandardControl<IInputs, IOutputs> {

	private _labelElement: HTMLLabelElement;
	private _checkBoxElement: HTMLInputElement;
	private _container: HTMLDivElement;
	private _notifyOutputChanged: () => void;

	private toggle: IToggle;
	/**
	 * Empty constructor.
	 */
	constructor()
	{

	}

	/**
	 * Used to initialize the control instance. Controls can kick off remote server calls and other initialization actions here.
	 * Data-set values are not initialized here, use updateView.
	 * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to property names defined in the manifest, as well as utility functions.
	 * @param notifyOutputChanged A callback method to alert the framework that the control has new outputs ready to be retrieved asynchronously.
	 * @param state A piece of data that persists in one session for a single user. Can be set at any point in a controls life cycle by calling 'setControlState' in the Mode interface.
	 * @param container If a control is marked control-type='standard', it will receive an empty div element within which it can render its content.
	 */
	public init(context: ComponentFramework.Context<IInputs>, notifyOutputChanged: () => void, state: ComponentFramework.Dictionary, container:HTMLDivElement): void
	{
		// Add control initialization code

		this._notifyOutputChanged = notifyOutputChanged;
		this._container = document.createElement("div");

		//toggle init
		this.toggle = {
			defaultValue: context.parameters.sampleProperty.attributes?.DefaultValue,
			currentValue: context.parameters.sampleProperty.raw,
			trueLable: context.parameters.sampleProperty.attributes?.Options[1].Label ?? "Yes",
			falseLabel: context.parameters.sampleProperty.attributes?.Options[0].Label ?? "No"
		};

		//set toggle current value
		if (this.toggle.currentValue == undefined && this.toggle.defaultValue != null) {
			this.toggle.currentValue = this.toggle.defaultValue;
        }

		//create html elements
		this._checkBoxElement = document.createElement("input");
		this._checkBoxElement.setAttribute("id", "ToggleId");
		this._labelElement = document.createElement("label");
		this._labelElement.setAttribute("id", "LabelId");

		this._checkBoxElement.type = "checkbox";
		this._checkBoxElement.checked = this.toggle.currentValue as boolean;

		this._checkBoxElement.addEventListener('change', (event) => {
			(event.target && (event.target as HTMLInputElement).checked) ? this.toggle.currentValue = true : this.toggle.currentValue = false;
			this._labelElement.innerText = this.toggle.currentValue ? this.toggle.trueLable : this.toggle.falseLabel;
			this._notifyOutputChanged();
		});

		this._container.appendChild(this._checkBoxElement);
		this._labelElement.innerText = this.toggle.currentValue ? this.toggle.trueLable : this.toggle.falseLabel;
		this._container.appendChild(this._labelElement);
		container.appendChild(this._container);
	}


	/**
	 * Called when any value in the property bag has changed. This includes field values, data-sets, global values such as container height and width, offline status, control metadata values such as label, visible, etc.
	 * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to names defined in the manifest, as well as utility functions
	 */
	public updateView(context: ComponentFramework.Context<IInputs>): void
	{
		// Add code to update control view
		if (context.parameters.sampleProperty.raw != this.toggle.currentValue) {
			this.toggle.currentValue = context.parameters.sampleProperty.raw;
			this._checkBoxElement.checked = this.toggle.currentValue;
			this._labelElement.textContent = this.toggle.currentValue ? this.toggle.trueLable : this.toggle.falseLabel;
        }
	}

	/**
	 * It is called by the framework prior to a control receiving new data.
	 * @returns an object based on nomenclature defined in manifest, expecting object[s] for property marked as “bound” or “output”
	 */
	public getOutputs(): IOutputs
	{
		return {
			sampleProperty: this.toggle.currentValue
		};
	}

	/**
	 * Called when the control is to be removed from the DOM tree. Controls should use this call for cleanup.
	 * i.e. cancelling any pending remote calls, removing listeners, etc.
	 */
	public destroy(): void
	{
		// Add code to cleanup control if necessary
	}
}
