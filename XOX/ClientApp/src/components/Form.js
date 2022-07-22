import React, { useState, useEffect, useRef } from 'react';
import EmojiPicker from './EmojiPicker';

export const Form = ({ user, onSubmit }) => {

    let [open, setOpen] = useState(false);
    let [mark, setMark] = useState(user.Mark);
    const wrapperRef = useRef("emojiMenu");

    useEffect(() => {
        /**
         * Invoke Function onClick outside of element
         */
        function handleClickOutside(event) {
            if (wrapperRef.current && !wrapperRef.current.contains(event.target)) {
                setOpen(false);
            }
        }
        // Bind
        document.addEventListener("mousedown", handleClickOutside);
        return () => {
            // dispose
            document.removeEventListener("mousedown", handleClickOutside);
        };
    });

    const onEmojiSelect = e => {
        setOpen(false);
        setMark(e.native);
    }

    const onMarkChange = e => {
        setOpen(false);
        setMark(e.native);
    }

    return (
        <div>
            <form id='form' onSubmit={onSubmit}>
                <div id='form-data'>
                    <div className="form-group">
                        <label htmlFor="name">Name</label>
                        <label htmlFor="mark">Mark</label>
                    </div>

                    <div className="form-group">
                        <input className="form-control" id="name" defaultValue={user.Name} maxLength="50" />
                        <input className="form-control" id="mark" value={mark} onClick={() => setOpen(true)} onChange={onMarkChange} maxLength="1" />
                    </div>

                </div>
                <div className="form-group">
                    <button className="form-control btn btn-primary" type="submit">
                        Submit
                    </button>
                </div>
            </form>
            <div id='emoji-picker' ref={wrapperRef}>
                {open &&
                    <EmojiPicker id="picker" onEmojiSelect={onEmojiSelect} />}
            </div>
        </div>
    );
};

export default Form;