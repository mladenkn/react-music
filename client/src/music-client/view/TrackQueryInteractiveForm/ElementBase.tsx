import React, { ReactElement } from "react";
import { makeStyles } from "@material-ui/styles";
import { ems } from "../../../utils/css";
import { Paper, FormGroup, IconButton } from "@material-ui/core";
import HighlightOffIcon from "@material-ui/icons/HighlightOff";
import { useImmer } from "use-immer";

const useStyles = makeStyles(() => ({
  removeButton: {
    padding: ems(0.2),
    position: "absolute",
    right: 0,
    top: 0
  }
}));

interface Props {
  className?: string;
  children: ReactElement | ReactElement[];
  onRemove: () => void
}

const useLogic = () => {
  const [state, updateState] = useImmer({
    isHovered: false
  });

  const onHover = () =>
    updateState(draft => {
      draft.isHovered = true;
    });

  const onHoverEnd = () =>
    updateState(draft => {
      draft.isHovered = false;
    });

  return { isHovered: state.isHovered, onHover, onHoverEnd };
};

export const ElementBase = (props: Props) => {
  const classes = useStyles();
  const logic = useLogic();

  return (
    <FormGroup row>
      <Paper
        onMouseEnter={logic.onHover}
        onMouseLeave={logic.onHoverEnd}
        className={props.className}
      >
        {logic.isHovered && (
          <IconButton onClick={props.onRemove} className={classes.removeButton}>
            <HighlightOffIcon />
          </IconButton>
        )}
        {props.children}
      </Paper>
    </FormGroup>
  );
};
