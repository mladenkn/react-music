import React, { ReactElement } from "react";
import { makeStyles } from "@material-ui/styles";
import { ems } from "../../../utils/css";
import { Paper, FormGroup, IconButton } from "@material-ui/core";
import HighlightOffIcon from "@material-ui/icons/HighlightOff";
import { useImmer } from "use-immer";
import clsx from "clsx";

const useStyles = makeStyles(() => ({
  root: {
    marginBottom: ems(1),
  },
  paper: {
    position: 'relative',
  },
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
  const styles = useStyles();
  const logic = useLogic();

  return (
    <FormGroup row className={styles.root}>
      <Paper
        onMouseEnter={logic.onHover}
        onMouseLeave={logic.onHoverEnd}
        className={clsx(props.className, styles.paper)}
      >
        {logic.isHovered && (
          <IconButton onClick={props.onRemove} className={styles.removeButton}>
            <HighlightOffIcon />
          </IconButton>
        )}
        {props.children}
      </Paper>
    </FormGroup>
  );
};
