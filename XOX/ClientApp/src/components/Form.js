import React from 'react';

export const Form = ({ onSubmit }) => {
    return (
        <form onSubmit={ onSubmit }>
            <div className="form-group">
                <label htmlFor="name">Name</label>
                <input className="form-control" id="name" />
            </div>

            <div className="form-group">
                <label htmlFor="mark">Mark</label>
                <input className="form-control" id="mark"
                    placeholder="X"
                />
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