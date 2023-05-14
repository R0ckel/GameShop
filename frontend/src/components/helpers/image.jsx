import React, {useEffect, useState} from 'react';
import userIcon from '../../image/user-icon.png'

const Image = ({ imageClassName, containerClassName, src }) => {
  const [imageSrc, setImageSrc] = useState(userIcon.toString())

  useEffect(()=>{
    setImageSrc(src)
  }, [src])

  const handleError = () => {
    setImageSrc(userIcon);
  };

  return (
    <div className={containerClassName}>
      <img src={imageSrc} className={imageClassName} alt="img" onError={handleError} />
    </div>
  );
};

export default Image