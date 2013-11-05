using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using InterpriseSuiteEcommerceControls.Validators;
using InterpriseSuiteEcommerceControls.Validators.Special;

public class BaseUserControl : System.Web.UI.UserControl
{
    #region Variable Declaration

    protected const string APPEARANCE_CATEGORY = "Address Appearance";
    protected const string VALIDATORS_CATEGORY = "Validators Category";

    private const string CAPTION_WIDTH = "CaptionWidth";
    private const string INPUT_WIDTH = "InputWidth";
    private const string STATE_TEMP = "StateTemporary";
    private const string TABLE_STYLE = "Table Style";
    private const string TABLE_CSS_CLASS = "TableClass";

    private InputValidatorSummary _errorSummaryControl;

    #endregion

    #region Constructor

    protected BaseUserControl()
    {

    }

    #endregion

    #region Properties

    [Browsable(false)]
    public InputValidatorSummary ErrorSummaryControl
    {
        get { return _errorSummaryControl; }
        set { _errorSummaryControl = value; }
    }

    #endregion

    protected class FormFieldBuilder
    {
        private Table _template = null;
        private VerticalAlign _defaultVerticalAlignment = VerticalAlign.Top;
        private TableRow _currentRow = null;

        private const string ALIGN_ATTRIBUTE = "align";
        public const string ALIGN_RIGHT = "right";
        public const string ALIGN_LEFT = "left";
        public const string ALIGN_CENTER = "center";

        public FormFieldBuilder(Table template)
        {
            _template = template;
        }

        #region Properties

        public Table Template
        {
            get { return _template; }
        }

        public VerticalAlign DefaultVerticalAlignment
        {
            get { return _defaultVerticalAlignment; }
            set { _defaultVerticalAlignment = value; }
        }

        public TableRow CurrentRow
        {
            get { return _currentRow; }
            set { _currentRow = value; }
        }

        #endregion

        #region Methods

        public void NewRow()
        {
            _currentRow = new TableRow();
        }

        public void CommitRow()
        {
            _template.Rows.Add(_currentRow);
        }

        public TableCell AddCell(string withAlignment, Unit forWidth, params Control[] withControls)
        {
            return AddCell(this.DefaultVerticalAlignment, withAlignment, forWidth, withControls);
        }

        public TableCell AddCell(VerticalAlign withVerticalAlignment, string withAlignment, Unit forWidth, params Control[] withControls)
        {
            if (null == _currentRow) { throw new InvalidOperationException("No Current Row!!!"); }

            TableCell cell = new TableCell();
            cell.VerticalAlign = withVerticalAlignment;
            cell.Attributes.Add(ALIGN_ATTRIBUTE, withAlignment);
            if (Unit.Empty != forWidth) { cell.Width = forWidth; }
            foreach (Control inputControl in withControls)
            {
                cell.Controls.Add(inputControl);
            }
            _currentRow.Cells.Add(cell);

            return cell;
        }

        #endregion
    }

    protected override void RenderChildren(HtmlTextWriter writer)
    {
        LoadAndAssignValidators();
        base.RenderChildren(writer);
    }

    //protected override override void CreateChildControls()
    //{
    //    LoadAndAssignValidators();
    //}

    private void LoadAndAssignValidators()
    {
        var validators = ProvideValidators();
        if (validators.Count > 0)
        {
            foreach (InputValidator validator in validators)
            {
                this.Controls.Add(validator);
            }
        }
    }

    protected virtual List<InputValidator> ProvideValidators()
    {
        return new List<InputValidator>();
    }

    protected virtual void HandleValidationErrorEvent(InputValidator validator)
    {
        if (null != this.ErrorSummaryControl)
        {
            validator.Error += this.ErrorSummaryControl.HandleValidationErrorEvent;
        }
    }

    #region MakeRequiredFieldValidator

    protected RequiredInputValidator MakeRequiredInputValidator(Control forControl, string withErrorMessage)
    {
        return MakeRequiredInputValidator(forControl, withErrorMessage, null);
    }

    protected RequiredInputValidator MakeRequiredInputValidator(Control forControl, string withErrorMessage, InputValidator next)
    {
        RequiredInputValidator validator = new RequiredInputValidator(forControl, withErrorMessage, next);
        HandleValidationErrorEvent(validator);

        return validator;
    }

    #endregion

    #region MakeCompareFieldValidator

    protected CompareInputValidator MakeCompareInputValidator(TextBox forControl, TextBox compareWithControl, string withErrorMessage)
    {
        return MakeCompareInputValidator(forControl, compareWithControl, withErrorMessage, null);
    }

    protected CompareInputValidator MakeCompareInputValidator(TextBox forControl, TextBox compareWithControl, string withErrorMessage, InputValidator next)
    {
        CompareInputValidator validator = new CompareInputValidator(forControl, compareWithControl, withErrorMessage, next);
        HandleValidationErrorEvent(validator);

        return validator;
    }

    #endregion

    #region MakeInputLengthValidator

    protected InputLengthValidator MakeInputLengthValidator(TextBox forControl, int withMaxLength, string withErrorMessage)
    {
        return MakeInputLengthValidator(forControl, InputLengthValidator.NoMinLengthFilter, withMaxLength, withErrorMessage);
    }

    protected InputLengthValidator MakeInputLengthValidator(TextBox forControl, int withMinLength, int withMaxLength, string withErrorMessage)
    {
        return MakeInputLengthValidator(forControl, withMinLength, withMaxLength, withErrorMessage, null);
    }

    protected InputLengthValidator MakeInputLengthValidator(TextBox forControl, int withMinLength, int withMaxLength, string withErrorMessage, InputValidator next)
    {
        InputLengthValidator validator = new InputLengthValidator(forControl, withMinLength, withMaxLength, withErrorMessage, next);
        HandleValidationErrorEvent(validator);

        return validator;
    }

    #endregion

    #region MakeRegularExpressionValidator

    protected RegularExpressionInputValidator MakeRegularExpressionInputValidator(TextBox forControl, string withRegularExpressionToMatchWith, string withErrorMessage)
    {
        return MakeRegularExpressionInputValidator(forControl, withRegularExpressionToMatchWith, withErrorMessage, null);
    }

    protected RegularExpressionInputValidator MakeRegularExpressionInputValidator(TextBox forControl, string withRegularExpressionToMatchWith, string withErrorMessage, InputValidator next)
    {
        RegularExpressionInputValidator validator = new RegularExpressionInputValidator(forControl, withRegularExpressionToMatchWith, withErrorMessage, next);
        HandleValidationErrorEvent(validator);

        return validator;
    }

    #endregion
}