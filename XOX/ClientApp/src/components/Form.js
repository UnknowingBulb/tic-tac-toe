import React, { useState, useEffect, useRef } from 'react';
import EmojiPicker from './EmojiPicker';

function ClickOutside(ref, onClickOutside) {
    useEffect(() => {
        /**
         * Invoke Function onClick outside of element
         */
        function handleClickOutside(event) {
            if (ref.current && !ref.current.contains(event.target)) {
                onClickOutside();
            }
        }
        // Bind
        document.addEventListener("mousedown", handleClickOutside);
        return () => {
            // dispose
            document.removeEventListener("mousedown", handleClickOutside);
        };
    }, [ref, onClickOutside]);
}
export const Form = ({ user, onSubmit }) => {

    let [open, setOpen] = useState(false);
    const wrapperRef = useRef("menu");
    ClickOutside(wrapperRef, () => {
        setOpen(false);
    });

    const onEmojiSelect = (event) => {
        console.log(event);
        setOpen(false);
    }

    return (
        <form onSubmit={onSubmit}>
            <div className="form-group">
                <label htmlFor="name">Name</label>
                <input className="form-control" id="name" defaultValue={user.Name} />
            </div>

            <div className="form-group">
                <label htmlFor="mark">Mark</label>
                <input className="form-control" id="mark" defaultValue={user.Mark} onClick={() => { console.log("fu"); setOpen(true) }} />
            </div>
            <div ref={wrapperRef}>
                {open &&
                    <EmojiPicker id='picker' onEmojiSelect={(event) => { console.log("rrr"); onEmojiSelect(event) }} />}
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