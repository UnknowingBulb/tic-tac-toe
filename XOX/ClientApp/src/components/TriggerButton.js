import React from 'react';

const TriggerButton = ({ buttonRef, showModal }) => {
    return (
        <button className="btn tech-btn" ref={buttonRef} onClick={showModal}>
            ⚙
        </button>
    );
};
export default TriggerButton;