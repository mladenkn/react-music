import React from 'react'
import { HomeSectionOptions, TrackQueryFormType } from "../shared/homeSectionOptions"
import { InputLabel, Select, MenuItem, Switch, TextField, makeStyles, createStyles, Button, Checkbox } from "@material-ui/core"
import { MusicDbTrackQueryInteractiveForm } from './MusicDbTrackQueryInteractiveForm'
import { useFormik } from 'formik'
import clsx from 'clsx'
import { ems, percent } from '../../utils/css'
import { snapshot } from '../../utils'
import { IdWithName } from '../../utils/types'
import { useEffect } from '../../utils/useEffect'
import { Track } from '../shared/track'

interface HomeSectionOptionsUIProps {
	className?: string
	values: HomeSectionOptions
	onChange: (f: HomeSectionOptions) => void
  onSearch: () => void
  tags: string[]
  youTubeChannels: IdWithName[]
  getTracksWithIds(ids: number[]): Track[]
}

const useStyles = makeStyles(
	() => createStyles({
		root: {
			display: 'flex',
			flexDirection: 'column',
			height: 'fit-content',
		},
		dataSource: {
		},
		fields: {
			marginTop: ems(1),
		},
		searchQueryField: {
			marginTop: ems(0.5),
			maxWidth: percent(83),
		},
		input: {
			display: 'flex',
			alignItems: 'center',
    },
    autoRefreshInput: {
      marginTop: ems(1),
    },
		searchButton: {			
			width: ems(5.5),
			alignSelf: 'flex-end',
		}
	}), {name: 'TrackQueryFormUi'}
)

export const HomeSectionOptionsUI = (props: HomeSectionOptionsUIProps) => {

	const styles = useStyles()
	const form = useFormik({
		enableReinitialize: true,
		initialValues: props.values,
		onSubmit: () => {}
	})
	useEffect(() => {
		console.log(form)
		props.onChange(form.values)
	}, [form.values])
	
	console.log(form.values.tracklist.query, snapshot(form.values), snapshot(props.values))

	return (
		<div className={clsx(props.className, styles.root)}>
			<Select
				className={styles.dataSource}
				label='Data source'
				value={form.values.tracklist.query.type}
				onChange={e => form.setFieldValue('tracklist.query.type', e.target.value)}
			>
				<MenuItem value={TrackQueryFormType.MusicDbQuery}>Music DB</MenuItem>
				<MenuItem value={TrackQueryFormType.YouTubeQuery}>YouTube</MenuItem>
			</Select>

			{form.values.tracklist.query.type === TrackQueryFormType.MusicDbQuery &&
				<MusicDbTrackQueryInteractiveForm
					className={styles.fields}
					input={form.values.tracklist.query.musicDbQuery!}
          onChange={value => form.setFieldValue('tracklist.query.musicDbQuery', value)}
          availableTags={props.tags}
          availableYouTubeChannels={props.youTubeChannels}
					getTracksWithIds={props.getTracksWithIds}
				/>
			} 

			{form.values.tracklist.query.type === TrackQueryFormType.YouTubeQuery &&
				<TextField
					className={styles.searchQueryField}
					label='Search Query'
					value={form.values.tracklist.query.youTubeQuery!}
					onChange={e => form.setFieldValue('tracklist.query.youTubeQuery', e.target.value)}
				/>
			}
			
			<div className={clsx(styles.input, styles.autoRefreshInput)}>
				<InputLabel>Auto refresh:</InputLabel>
				<Switch 
					checked={form.values.tracklist.autoRefresh} 
					onChange={e => form.setFieldValue('tracklist.autoRefresh', e.target.checked)}
					color='primary'
				/>
			</div>
			
			<div className={styles.input}>
				<InputLabel>Auto play:</InputLabel>
				<Switch 
					checked={form.values.tracklist.autoPlay} 
					onChange={e => form.setFieldValue('tracklist.autoPlay', e.target.checked)}
					color='primary'
				/>
			</div>
			
			<div className={styles.input}>
				<InputLabel>Tracklist shown:</InputLabel>
				<Checkbox
					checked={form.values.tracklistShown}
					color='primary'
					onChange={e => form.setFieldValue('tracklistShown', e.target.checked)}
				/>
			</div>

			<Button className={styles.searchButton} color="primary" onClick={props.onSearch}>Search</Button>
		</div>
	)
}
