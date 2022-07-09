import React, { useEffect, useRef } from 'react'
import data from '@emoji-mart/data'
import { Picker } from 'emoji-mart'

const EmojiPicker = props => {
    const ref = useRef();

    useEffect(() => {
        new Picker({ ...props, data, ref })
    }, []);

    const styles = {
        emojis: { textAlign: 'center' },
    }

    return <div ref={ref} style={styles.emojis}></div>
}
export default EmojiPicker;