import { makeStyles, Theme } from "@material-ui/core"
import React from 'react'
import clsx from 'clsx'
import YouTube from 'react-youtube'

const useTrackPlayerStyles = makeStyles<Theme, StyleProps>(() => ({
	root: ({ width, height }) => ({
    width, 
    height
	}),
}), {name: 'TrackPlayer'})

type StyleProps = {
  width: number | string
  height: number | string
}

type TrackPlayerProps = {
  className?: string
  videoId: string
  playImmediately: boolean
  onTrackEnd(): void
} & StyleProps

export const TrackPlayerUI = (p: TrackPlayerProps) => {
	const classes = useTrackPlayerStyles(p)
	return (
    <YouTube 
      onEnd={p.onTrackEnd}
      videoId={p.videoId} 
      opts={{
        width: (typeof p.width === 'number' ?  p.width.toString() : p.width),
        height: (typeof p.height === 'number' ?  p.height.toString() : p.height),
        playerVars: { // https://developers.google.com/youtube/player_parameters
          autoplay: p.playImmediately ? 1 : 0
        }
      }}
      className={clsx(classes.root, p.className)} 
    />
	)
}
