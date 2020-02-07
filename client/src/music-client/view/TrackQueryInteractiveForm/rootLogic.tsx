import { TrackQueryFromProps } from ".";
import { useImmer } from "use-immer";
import { difference } from "lodash";

const { usePopupState } = require("material-ui-popup-state/hooks");

export interface TrackDataForm {
  titleMatch: string | undefined;
  channel: string | undefined;
  mustContainAllTags: string[];
  mustContainSomeTags: string[];
  yearSpan?: {
    from?: number;
    to?: number;
  };
}

export type Field = keyof TrackDataForm;

interface State {
  activeFields: Field[];
}

const nullableFields: Field[] = ["titleMatch", "channel"];
const arrayFields: Field[] = [
  "mustContainAllTags",
  "mustContainSomeTags"
];

const avaiableFields: Field[] = [
  //"channel",
  "mustContainAllTags",
  // "mustContainSomeTags",
  //"titleMatch",
  "yearSpan"
];

const getDefaultFieldValue = (field: Field) => {
  if (nullableFields.includes(field)) return undefined;
  else if (arrayFields.includes(field)) return [];
  else if (field === 'yearSpan') return {};
};

export const useRootLogic = (p: TrackQueryFromProps) => {
  const [state, updateState] = useImmer<State>({
    activeFields: []
  });

  const mapInput = (yearSpanTo: "incr" | "decr") => (i: TrackDataForm) => {
    const num = yearSpanTo === "incr" ? 1 : -1;
    const mappedYearSpan = i.yearSpan && {
      from: i.yearSpan.from,
      to: i.yearSpan!.to && i.yearSpan!.to + num
    };
    return { ...i, yearSpan: mappedYearSpan };
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
