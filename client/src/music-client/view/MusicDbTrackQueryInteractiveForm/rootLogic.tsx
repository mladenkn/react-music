import { MusicDbTrackQueryInteractiveFormProps } from ".";
import { useImmer } from "use-immer";
import { difference } from "lodash";
import { MusicDbTrackQueryForm } from "../../shared";

const { usePopupState } = require("material-ui-popup-state/hooks");

export type Field = keyof MusicDbTrackQueryForm;

interface State {
  activeFields: Field[];
}

const nullableFields: Field[] = ["titleContains"];
const arrayFields: Field[] = [
  "mustHaveEveryTag",
  "mustHaveAnyTag"
];

const avaiableFields: Field[] = [
  "mustHaveEveryTag",
  "mustHaveAnyTag",
  "titleContains",
  'yearRange'
];

const getDefaultFieldValue = (field: Field) => {
  if (nullableFields.includes(field)) return undefined;
  else if (arrayFields.includes(field)) return [];
  else if (field === 'yearRange') return {};
};

export const useRootLogic = (p: MusicDbTrackQueryInteractiveFormProps) => {
  const [state, updateState] = useImmer<State>({
    activeFields: []
  });

  const mapInput = (yearSpanTo: "incr" | "decr") => (i: MusicDbTrackQueryForm): MusicDbTrackQueryForm => {
    const num = yearSpanTo === "incr" ? 1 : -1;
    const mappedYearSpan = i.yearRange && {
      lowerBound: i.yearRange.lowerBound,
      upperBound: i.yearRange!.upperBound && i.yearRange!.upperBound + num
    };
    return { ...i, yearRange: mappedYearSpan };
  };

  // const formLogic = useFormLogic(
  //   p.input,
  //   p.onChange,
  //   mapInput("incr"),
  //   mapInput("decr")
  // );

  const setFieldActive = (field: Field) => {
    popupState.close()
    updateState(draft => {
      draft.activeFields.push(field);
    });
  };

  const setFieldInactive = (field: Field) => {
    // formLogic.onPropChange(field)(getDefaultFieldValue(field));
    updateState(draft => {
      draft.activeFields = draft.activeFields.filter(f => f !== field);
    });
  };

  const isFieldActive = (field: Field) => state.activeFields.includes(field);

  const inactiveFields = difference(avaiableFields, state.activeFields);

  const availableTags = [
    "trance",
    "techno",
    "house",
    "smirujuće",
    "tralalal",
    "sldkfjsdlf",
    "sdfsdf"
  ];

  const popupState = usePopupState({ variant: "popover", popupId: "TrackQueryInteractiveFormPopup" });

  return {
    // form: formLogic,
    availableTags,
    popupState,
    inactiveFields,
    setFieldActive,
    setFieldInactive,
    isFieldActive,
  };
};
