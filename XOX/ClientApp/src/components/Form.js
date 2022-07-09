import React, { useState, useEffect, useRef } from 'react';
import EmojiPicker from './EmojiPicker';

export const Form = ({ user, onSubmit }) => {

    let [open, setOpen] = useState(false);
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
        // Close the picker modal
        setOpen(false);
        //e.name.value = e.native;
        console.log(e.native);
    }

    return (
        <form onSubmit={onSubmit}>
            <div className="form-group">
                <label htmlFor="name">Name</label>
                <input className="form-control" id="name" defaultValue={user.Name} />
            </div>

            <div className="form-group">
                <label htmlFor="mark">Mark</label>
                <input className="form-control" id="mark" defaultValue={user.Mark} onClick={() => setOpen(true)} />
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