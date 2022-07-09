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
        <form onSubmit={onSubmit}>
            <div className="form-group">
                <label htmlFor="name">Name</label>
                <input className="form-control" id="name" defaultValue={user.Name} maxLength="50" />
            </div>

            <div className="form-group">
                <label htmlFor="mark">Mark</label>
                <input className="form-control" id="mark" value={mark} onClick={() => setOpen(true)} onChange={onMarkChange} maxLength="1" />
            </div>
            <div ref={wrapperRef}>
                {open &&
                    <EmojiPicker id="picker" onEmojiSelect={onEmojiSelect} />}
            </div>
            <div className="form-group">
                <button className="form-control btn btn-primary" type="submit">
                    Submit
                </button>
            </div>
        </form>
    );
};

export default Form;