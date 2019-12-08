import React from 'react'
import { ButtonBase } from "@material-ui/core";
import { useUserKeyLogic } from "../logic/userKey";
import { TextInputDialog } from "../../utils/components/TextInputDialog";
import { useState } from "react";

export const UserKeyInput = (p: {className: string}) => {
  const userKeyLogic = useUserKeyLogic()
  const [clicksCount, setClicksCount] = useState(0)
  const [userClosedDialog, setUserClosedDialog] = useState(false)
  const isDialogOpen = ((clicksCount !== 0) && (clicksCount % 3 === 0)) && !userClosedDialog
  return (
    <ButtonBase disableRipple id='UserKeyInputButton' className={p.className} onClick={e => {
      const sourceId = (e.target as any).id as string
      if(sourceId === 'UserKeyInputButton')
        setClicksCount(clicksCount + 1)
    }}>
      <TextInputDialog 
        initialInput={userKeyLogic.key}
        heading={'Enter your key:'}
        onOK={newKey => {
          userKeyLogic.setKey(newKey)
          setUserClosedDialog(true)
        }} 
        onCancel={() => setUserClosedDialog(true)}
        isOpen={isDialogOpen}
      />
    </ButtonBase>
    )
}

