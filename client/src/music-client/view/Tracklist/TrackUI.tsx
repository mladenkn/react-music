import { percent, ems } from "../../../utils/css";
import { createStyles, withStyles } from "@material-ui/styles";
import React, { useState, Fragment } from "react";
import { Card, IconButton, Typography, Dialog, WithStyles } from "@material-ui/core"
import { TrackEditablePropsEditUI } from "./TrackEditablePropsEdit"
import EditIcon from "@material-ui/icons/Edit"
import SaveIcon from "@material-ui/icons/Save"
import CancelIcon from "@material-ui/icons/Cancel"
import DescriptionIcon from "@material-ui/icons/Description"
import PlayCircleOutlinedIcon from "@material-ui/icons/PlayCircleOutlineOutlined"
import { TrackEditablePropsReadonlyUI } from "./TrackEditablePropsReadonly";
import discogsIconUrl from '../icons/discogs.png'
import youtubeIconUrl from '../icons/youtube.png'
import recommendationUrl from '../icons/recommendation2.png'
import { Link } from "../../../utils/view";
import clsx from 'clsx';
import { $PropertyType } from "utility-types";
import { TrackViewModel, TrackEditableProps, Track } from "../../shared/track";
import { useImmer } from "use-immer";

const styles = createStyles({
  paper: {
    padding: ems(0.5, 1),
    width: percent(100),
  },  
  content: {
    display: 'flex',
  },
  firstRow: {
    display: 'flex',
    width: percent(100),
  },
  left: {

  },
  right: {
    marginLeft: ems(0.5)
  },
  actions: {
    display: 'inline-block',
  },
  image: {
    width: ems(10),
  },
  data: {
    minHeight: ems(4),
  }, 
  props: {
    padding: ems(0.4),
  },
  propsEdit: {
    margin: ems(1, 0),
  },
  heading: {
    fontSize: ems(1.5),
    marginBottom: ems(0.5),
  },
  channelTitle: {

  },
  normalFontSize: {
    fontSize: ems(1),
  },
  actionIcon: {
    fontSize: ems(0.9),
    height: ems(1),
  },
  actionButton: {
    padding: ems(0.3),
  },
  recommendationActionButton: {
    marginBottom: ems(0.1),
  },
  descriptionModal: {
    width: ems(30),
    padding: ems(0.5, 0.8),
  }
})

type ItemProps = {
  className?: string
  track: TrackViewModel
  onPlay: () => void
  fetchRecommendationsOf: (trackId: number) => void
  onClick: () => void
  isFocused: boolean
  saveTrack(t: TrackEditableProps): Promise<void>
} & WithStyles<typeof styles>

export type TrackUIClasses = Partial<$PropertyType<ItemProps, 'classes'>>

const useLogic = (onFinsihEdit: (t: TrackEditableProps) => Promise<void>, initial: TrackEditableProps) => {
  const [state, updateState] = useImmer({
    isEdit: false,
    descriptionModalOpen: false,
    editedProps: initial
  })

  const triggerEdit = () => updateState(draft => {
    draft.isEdit = true
  });  
  const cancelEdit = () => updateState(draft => {
    draft.isEdit = false
  });

  const finishEdit = async () => {
    await onFinsihEdit(state.editedProps!);
    updateState(draft => {
      draft.isEdit = false
    })
  }
  const setEditedProps = (editedProps: TrackEditableProps) => updateState(draft => {
    draft.editedProps = editedProps
  });

  const toogleDescriptionModalOpen = () => updateState(draft => {
    draft.descriptionModalOpen = !draft.descriptionModalOpen  
  });

  return { 
    editedProps: state.editedProps,
    isEdit: state.isEdit, 
    setEditedProps, 
    triggerEdit, 
    finishEdit, 
    cancelEdit, 
    descriptionModalOpen: state.descriptionModalOpen, 
    toogleDescriptionModalOpen 
  }
}
 
const TrackUI_ = (p: ItemProps) => {
  const logic = useLogic(p.saveTrack, p.track)

	return (
    <Fragment>
      <Card className={p.classes.paper} onClick={p.onClick} raised={p.isFocused}>
        <Typography className={p.classes.heading}>{p.track.title}</Typography>
        <div className={p.classes.content}>
          <div className={p.classes.left}>
            <img className={p.classes.image} src={p.track.image} />
          </div>
          <div className={p.classes.right}>
            <div className={p.classes.data}>
              <div className={p.classes.props}>              
                <Typography className={p.classes.normalFontSize}>{p.track.youtubeChannelTitle}</Typography>
                {logic.isEdit ?
                  <TrackEditablePropsEditUI
                    onChange={logic.setEditedProps}
                    track={logic.editedProps!}
                    className={p.classes.propsEdit} 
                    textClassName={p.classes.normalFontSize}
                  /> : 
                  <TrackEditablePropsReadonlyUI textClassName={p.classes.normalFontSize} track={p.track.editableProps} />
                }
              </div>
            </div>
            { p.isFocused &&
              <div className={p.classes.actions}>
                <IconButton className={p.classes.actionButton} onClick={logic.toogleDescriptionModalOpen}>
                  <DescriptionIcon className={p.classes.actionIcon} />
                </IconButton>
                {p.track.canFetchRecommendations && 
                  <IconButton className={clsx(p.classes.actionButton, p.classes.recommendationActionButton)} onClick={() => p.fetchRecommendationsOf(p.track.id)}>
                    <img src={recommendationUrl} className={p.classes.actionIcon} />
                  </IconButton>
                }
                {p.track.canEdit && !logic.isEdit && 
                  <IconButton className={p.classes.actionButton} onClick={logic.triggerEdit}>
                    <EditIcon className={p.classes.actionIcon} />
                  </IconButton>
                }
                {p.track.canEdit && logic.isEdit && 
                  <IconButton className={p.classes.actionButton} onClick={logic.finishEdit}>
                    <SaveIcon className={p.classes.actionIcon} />
                  </IconButton>
                }
                {p.track.canEdit && logic.isEdit && 
                  <IconButton className={p.classes.actionButton} onClick={logic.cancelEdit}>
                    <CancelIcon className={p.classes.actionIcon} />
                  </IconButton>
                }
                {p.track.canPlay && 
                  <IconButton className={p.classes.actionButton} onClick={p.onPlay}>
                    <PlayCircleOutlinedIcon className={p.classes.actionIcon} />
                  </IconButton>
                }
                <Link href={p.track.discogsSearchUrl}>
                  <IconButton className={p.classes.actionButton}>
                    <img src={discogsIconUrl} className={p.classes.actionIcon} />
                  </IconButton>
                </Link>
                <Link href={p.track.youtubeVideoUrl}>
                  <IconButton className={p.classes.actionButton}>
                    <img src={youtubeIconUrl} className={p.classes.actionIcon} />
                  </IconButton> 
                </Link>
              </div>
            }
          </div>
        </div>
      </Card>
      <Dialog open={logic.descriptionModalOpen} onClose={logic.toogleDescriptionModalOpen}>
        <div className={p.classes.descriptionModal}>          
          <Typography>{p.track.description}</Typography>
        </div>
      </Dialog>
    </Fragment>
	)
}

export const TrackUI = withStyles(styles)(TrackUI_)